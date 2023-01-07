using EodHistoricalData.Sdk;
using EodHistoricalData.Sdk.Models;
using EodHistoricalData.Sdk.Models.Fundamentals.CommonStock;
using EodHistoricalData.Sdk.Models.Fundamentals.Etf;
using Import.Infrastructure;
using Import.Infrastructure.Abstractions;
using EodHistoricalData.Sdk.Events;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static Import.Infrastructure.Configuration.Constants;

namespace Import.AppServices
{
    public sealed class DataImportService
    {
        private readonly ILogsDbContext logsDbContext;
        private readonly IImportsDbContext importsDbContext;
        private readonly IDataClient dataClient;
        private readonly ILogger? logger;
        private readonly HashSet<Symbol> allSymbols = new();

        public delegate void ApiLimitReachedHandler(object? sender, ApiLimitReachedException apiLimitReachedException);

        public event ApiLimitReachedHandler? ApiLimitReachedEventHandler;

        public delegate void ApiResponseExceptionHandler(object? sender,
            ApiResponseException apiResponseException,
            string[] symbols);

        public event ApiResponseExceptionHandler? ApiResponseExceptionEventHandler;

        internal DataImportService(ILogsDbContext logsDbContext,
            IImportsDbContext importsDbContext,
            string apiKey,
            int maxusage = 100_000,
            ILogger? logger = null)
            : this(logsDbContext, importsDbContext, new DataClient(apiKey, logger), maxusage, logger)
        { }

        internal DataImportService(ILogsDbContext logsDbContext,
            IImportsDbContext importsDbContext,
            IDataClient dataClient,
            int maxUsage = 100_000,
            ILogger? logger = null)
        {
            this.logsDbContext = logsDbContext;
            this.importsDbContext = importsDbContext;

            this.logger = logger;

            this.dataClient = dataClient;

            _ = dataClient.ResetUsageAsync(maxUsage).GetAwaiter().GetResult();

            SymbolsToIgnore.SetItems(importsDbContext.GetSymbolsToIgnoreAsync().GetAwaiter().GetResult().ToArray());

            SymbolMetaDataRepository.SetItems(importsDbContext.GetSymbolMetaDataAsync().GetAwaiter().GetResult().ToArray());

        }

        private void DataClient_ApiLimitReachedEventHandler(object sender, ApiLimitReachedException apiLimitReachedException)
        {
            ApiLimitReachedEventHandler?.Invoke(sender, apiLimitReachedException);
        }

        public static int Usage => ApiService.Usage;

        public static int DailyLimit => ApiService.DailyLimit;

        public Task PurgeDataAsync(string purgeName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (purgeName == PurgeName.Logs)
            {
                return logsDbContext.PurgeLogsAsync(cancellationToken);
            }

            if (purgeName == PurgeName.ActionItems)
            {
                return logsDbContext.PurgeActionItemsAsync(cancellationToken);
            }

            if (purgeName == PurgeName.Imports)
            {
                return importsDbContext.PurgeAsync(cancellationToken);
            }

            return Task.CompletedTask;
        }

        public async Task TruncateLogsAsync(string logLevel, DateTime date, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (date > DateTime.UtcNow) { await Task.CompletedTask; }

            await logsDbContext.TruncateLogsAsync(logLevel, date, cancellationToken);
        }

        public Task ImportDataAsync(string scope, string exchange, string dataType, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DomainEventPublisher.RaiseMessageEvent(this, $"Importing\t{scope} {exchange} {dataType}", nameof(ImportDataAsync));

            if (dataType == DataTypes.Symbols)
            {
                var symbols = dataClient.GetSymbolListAsync(exchange, cancellationToken)
                    .GetAwaiter().GetResult()
                    .ToArray();

                allSymbols.UnionWith(symbols);

                return importsDbContext.SaveSymbolsAsync(symbols, cancellationToken);
            }
            else if (scope == DataTypeScopes.Full)
            {
                return ImportFullAsync(exchange, dataType, cancellationToken);
            }
            else if (scope == DataTypeScopes.Bulk)
            {
                return ImportBulkAsync(exchange, dataType, cancellationToken);
            }

            return Task.CompletedTask;
        }

        public async Task ApplyFixAsync(string name, CancellationToken cancellationToken)
        {
            if (name.Equals("has options", StringComparison.OrdinalIgnoreCase))
            {
                var symbols = File.ReadAllLines("Fixes/OptionableSymbols.txt")
                    .Select(s => s.Trim());

                foreach (var chunk in symbols.Chunk(500))
                {
                    await importsDbContext.SetOptionableOnSymbolsAsync(chunk, cancellationToken);
                }
            }
        }

        private Task ImportFullAsync(string exchange, string dataType, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (exchange == dataType && dataType == DataTypes.Exchanges) // both Exchange and DateType are "Exchanges"
            {
                int estimatedCost = ApiService.GetCost(ApiService.ExchangesUri);
                if (estimatedCost > ApiService.Available)
                {
                    ApiLimitReachedEventHandler?.Invoke(this, new ApiLimitReachedException(
                        $"exchange list", ApiService.Usage, estimatedCost + ApiService.Usage));
                }
                else
                {
                    return importsDbContext.SaveExchangesAsync(dataClient.GetExchangeListAsync(cancellationToken).GetAwaiter().GetResult(),
                        cancellationToken);
                }

                return Task.CompletedTask;
            }

            if (!allSymbols.Any())
            {
                allSymbols.UnionWith(importsDbContext.GetAllSymbolsAsync(cancellationToken).GetAwaiter().GetResult());
            }

            var symbolsForExchange = allSymbols.Where(s => s.Exchange == exchange).Except(allSymbols.Where(s =>
                SymbolsToIgnore.IsOnList(s.Code, s.Exchange))).ToArray();

            if (dataType == DataTypes.Splits)
            {
                int estimatedCost = ApiService.GetCost(ApiService.SplitsUri, symbolsForExchange.Length);
                if (estimatedCost > ApiService.Available)
                {
                    ApiLimitReachedEventHandler?.Invoke(this, new ApiLimitReachedException(
                        $"splits for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage));
                }
                else
                {
                    return ImportSplitsAsync(symbolsForExchange, cancellationToken);
                }
            }

            if (dataType == DataTypes.Dividends)
            {
                int estimatedCost = ApiService.GetCost(ApiService.DividendUri, symbolsForExchange.Length);
                if (estimatedCost > ApiService.Available)
                {
                    ApiLimitReachedEventHandler?.Invoke(this, new ApiLimitReachedException(
                        $"dividends for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage));
                }
                else
                {
                    return ImportDividendsAsync(symbolsForExchange, cancellationToken);
                }
            }

            if (dataType == DataTypes.Prices)
            {
                int estimatedCost = ApiService.GetCost(ApiService.EodUri, symbolsForExchange.Length);
                if (estimatedCost > ApiService.Available)
                {
                    ApiLimitReachedEventHandler?.Invoke(this, new ApiLimitReachedException(
                        $"prices for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage));
                }
                else
                {
                    return ImportPricesAsync(symbolsForExchange, cancellationToken);
                }
            }

            if (dataType == DataTypes.Options)
            {
                var symbolsWithOptions = SymbolMetaDataRepository.Find(m => m.HasOptions);

                if (symbolsWithOptions.Any())
                {
                    int estimatedCost = ApiService.GetCost(ApiService.OptionsUri, symbolsForExchange.Length);
                    if (estimatedCost > ApiService.Available)
                    {
                        ApiLimitReachedEventHandler?.Invoke(this, new ApiLimitReachedException(
                            $"options for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage));
                    }
                    else
                    {
                        var symbols = importsDbContext.GetSymbolsWithOptionsAsync(cancellationToken)
                            .GetAwaiter().GetResult().ToArray();
                        return ImportOptionsAsync(symbols, cancellationToken);
                    }
                }
            }

            if (dataType == DataTypes.Fundamentals)
            {
                // find the symbols due for an update to their fundamentals (i.e., every 3 months).
                var meta = SymbolMetaDataRepository.Find(m => m.LastUpdatedIncomeStatement.GetValueOrDefault() <
                    DateTime.Now.AddDays(-90)).ToArray();

                int baseCost = ApiService.GetCost(ApiService.FundamentalsUri, 1);

                int estimatedCost = baseCost * meta.Length;

                int availableCycles = Math.Min(estimatedCost, ApiService.Available) / baseCost;

                if (meta.Any())
                {
                    for (int i = 0; i < availableCycles; i++)
                    {
                        ImportFundamentalsAsync(meta[i].Symbol, cancellationToken).GetAwaiter().GetResult();
                    }
                }

                if (ApiService.Usage >= ApiService.DailyLimit)
                {
                    ApiLimitReachedEventHandler?.Invoke(this, new ApiLimitReachedException(
                        $"fundamentals for {exchange}", ApiService.Usage, ApiService.Usage));
                }
            }

            return Task.CompletedTask;
        }

        private async Task ImportSplitsAsync(Symbol[] symbols, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var symbol in symbols)
            {
                var splits = (await dataClient.GetSplitsForSymbolAsync(symbol.Code, cancellationToken: cancellationToken)).ToList();

                List<Infrastructure.Domain.Split> domainSplits = new();

                splits.ForEach(s => domainSplits.Add(new Infrastructure.Domain.Split(symbol.Code, symbol.Exchange, s)));

                await importsDbContext.SaveSplitsAsync(domainSplits, cancellationToken);
            }
        }

        private async Task ImportDividendsAsync(Symbol[] symbols, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var symbol in symbols)
            {
                var divs = await dataClient.GetDividendsForSymbolAsync(symbol.Code, cancellationToken: cancellationToken);

                await importsDbContext.SaveDividendsAsync(symbol.Code, symbol.Exchange, divs, cancellationToken);
            }
        }

        private async Task ImportPricesAsync(Symbol[] symbols, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Parallel.ForEachAsync(symbols, new ParallelOptions()
            {
                MaxDegreeOfParallelism = 5,
                CancellationToken = cancellationToken
            }, async (symbol, t) =>
            {
                try
                {
                    var prices = await dataClient.GetPricesForSymbolAsync(symbol.Code, cancellationToken: cancellationToken);
                    await importsDbContext.SavePriceActionsAsync(symbol.Code, symbol.Exchange, prices, cancellationToken);
                }
                catch (JsonException jsonExc)
                {
                    logger?.LogError(jsonExc, "Could not parse price actions for '{SYMBOL}'", symbol);
                }
            });
        }

        private async Task ImportOptionsAsync(Symbol[] symbols, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var symbol in symbols)
            {
                var options = await dataClient.GetOptionsForSymbolAsync(symbol.Code, cancellationToken: cancellationToken);

                await importsDbContext.SaveOptionsAsync(options, cancellationToken);
            }
        }

        private Task ImportBulkAsync(string exchange, string dataType, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (exchange == dataType && dataType == DataTypes.Exchanges)
            {
                var modelExchanges = dataClient.GetExchangeListAsync(cancellationToken).GetAwaiter().GetResult();

                return importsDbContext.SaveExchangesAsync(modelExchanges, cancellationToken);
            }

            return Task.CompletedTask;
        }

        private Task ImportFundamentalsAsync(string symbol, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fundamentals = dataClient.GetFundamentalsForSymbolAsync(symbol, cancellationToken: cancellationToken)
                .GetAwaiter().GetResult();

            if (fundamentals is EtfFundamentalsCollection etfCollection)
            {
                return importsDbContext.SaveEtfAsync(etfCollection, cancellationToken);
            }

            if (fundamentals is FundamentalsCollection collection)
            {
                return importsDbContext.SaveCompanyAsync(collection, cancellationToken);
            }

            throw new Exception($"Could not import fundamentals for {symbol}");
        }

        private void DataClient_ApiResponseExceptionEventHandler(object sender, ApiResponseException apiResponseException, string[] symbols)
        {
            ApiResponseExceptionEventHandler?.Invoke(sender, apiResponseException, symbols);
        }
    }
}
