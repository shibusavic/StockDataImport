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
using Import.AppServices.Configuration;

namespace Import.AppServices
{
    public sealed class DataImportService
    {
        private readonly ILogger? logger;
        private readonly HashSet<Symbol> allSymbols = new();

        internal DataImportService(ILogsDbContext logsDbContext,
            IImportsDbContext importsDbContext,
            string apiKey,
            int maxUsage = 100_000,
            ILogger? logger = null)
            : this(logsDbContext, importsDbContext, new DataClient(apiKey, logger), maxUsage, logger)
        { }

        internal DataImportService(ILogsDbContext logsDbContext,
            IImportsDbContext importsDbContext,
            IDataClient dataClient,
            int maxUsage = 100_000,
            ILogger? logger = null)
        {
            LogsDb = logsDbContext;
            ImportsDb = importsDbContext;

            this.logger = logger;

            DataClient = dataClient;

            _ = DataClient.ResetUsageAsync(maxUsage).GetAwaiter().GetResult();

            SymbolsToIgnore.SetItems(ImportsDb.GetSymbolsToIgnoreAsync().GetAwaiter().GetResult().ToArray());

            SymbolMetaDataRepository.SetItems(ImportsDb.GetSymbolMetaDataAsync().GetAwaiter().GetResult().ToArray());

        }

        internal IDataClient DataClient { get; }

        internal ILogsDbContext LogsDb { get; }

        internal IImportsDbContext ImportsDb { get; }

        public static int Usage => ApiService.Usage;

        public static int DailyLimit => ApiService.DailyLimit;

        public Task PurgeDataAsync(string purgeName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (purgeName == PurgeName.Logs)
            {
                return LogsDb.PurgeLogsAsync(cancellationToken);
            }

            if (purgeName == PurgeName.ActionItems)
            {
                return LogsDb.PurgeActionItemsAsync(cancellationToken);
            }

            if (purgeName == PurgeName.Imports)
            {
                return ImportsDb.PurgeAsync(cancellationToken);
            }

            if (purgeName == PurgeName.ApiResponses)
            {
                return LogsDb.PurgeApiResponsesAsync(cancellationToken);
            }

            return Task.CompletedTask;
        }

        public async Task TruncateLogsAsync(string logLevel, DateTime date, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (date > DateTime.UtcNow) { await Task.CompletedTask; }

            await LogsDb.TruncateLogsAsync(logLevel, date, cancellationToken);
        }

        public async Task TruncateApiResponsesAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (date > DateTime.UtcNow) { await Task.CompletedTask; }

            await LogsDb.TruncateApiResponsesAsync(date, cancellationToken);
        }

        public async Task TruncateActionItemsAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (date > DateTime.UtcNow) { await Task.CompletedTask; }

            await LogsDb.TruncateActionItemsAsync(date, cancellationToken);
        }

        public Task ImportDataAsync(string scope, string exchange, string dataType,
            //IEnumerable<string>? exchangeIncludeFilters = null,
            //IEnumerable<string>? symbolTypeIncludeFilters = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //exchangeIncludeFilters ??= Enumerable.Empty<string>();
            //symbolTypeIncludeFilters ??= Enumerable.Empty<string>();

            DomainEventPublisher.RaiseMessageEvent(this, $"Importing\t{scope} {exchange} {dataType}", nameof(ImportDataAsync));

            if (dataType == DataTypes.Symbols)
            {
                var symbols = DataClient.GetSymbolListAsync(exchange, cancellationToken)
                    .GetAwaiter().GetResult()
                    .ToArray();

                HashSet<Symbol> symbolSet = new(symbols);

                //if (exchangeIncludeFilters.Any())
                //{
                //    symbolSet.RemoveWhere(s => !exchangeIncludeFilters.Contains(s.Exchange));
                //}

                //if (symbolTypeIncludeFilters.Any())
                //{
                //    symbolSet.RemoveWhere(s => !symbolTypeIncludeFilters.Contains(s.Type));
                //}
                
                allSymbols.UnionWith(symbolSet);

                return ImportsDb.SaveSymbolsAsync(symbols, exchange, cancellationToken);
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

        public async Task ApplyFixAsync(string name, CancellationToken cancellationToken = default)
        {
            if (name.Equals("has options", StringComparison.OrdinalIgnoreCase))
            {
                var symbols = File.ReadAllLines("Fixes/OptionableSymbols.txt")
                    .Select(s => s.Trim());

                foreach (var chunk in symbols.Chunk(500))
                {
                    await ImportsDb.SetOptionableOnSymbolsAsync(chunk, cancellationToken);
                }
            }
        }

        public Task SaveApiResponse(string request, string response, int statusCode,
            CancellationToken cancellationToken = default)
        {
            return LogsDb.SaveApiResponseAsync(request, response, statusCode, cancellationToken);
        }

        private Task ImportFullAsync(string exchange, string dataType, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (exchange == dataType && dataType == DataTypes.Exchanges) // both Exchange and DateType are "Exchanges"
            {
                int estimatedCost = ApiService.GetCost(ApiService.ExchangesUri);
                if (estimatedCost > ApiService.Available)
                {
                    DomainEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
                        $"exchange list", ApiService.Usage, estimatedCost + ApiService.Usage), nameof(ImportFullAsync));
                }
                else
                {
                    return ImportsDb.SaveExchangesAsync(DataClient.GetExchangeListAsync(cancellationToken).GetAwaiter().GetResult(),
                        cancellationToken);
                }

                return Task.CompletedTask;
            }

            if (!allSymbols.Any())
            {
                allSymbols.UnionWith(ImportsDb.GetAllSymbolsAsync(cancellationToken).GetAwaiter().GetResult());
            }

            var symbolsForExchange = allSymbols.Where(s => s.Exchange == exchange).Except(allSymbols.Where(s =>
                SymbolsToIgnore.IsOnList(s.Code, s.Exchange ?? "Unknown"))).ToArray();

            if (dataType == DataTypes.Splits)
            {
                int estimatedCost = ApiService.GetCost(ApiService.SplitsUri, symbolsForExchange.Length);
                if (estimatedCost > ApiService.Available)
                {
                    DomainEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
                        $"splits for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage), nameof(ImportFullAsync));
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
                    DomainEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
                        $"dividends for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage), nameof(ImportFullAsync));
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
                    DomainEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
                        $"prices for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage), nameof(ImportFullAsync));
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
                        DomainEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
                            $"options for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage), nameof(ImportFullAsync));
                    }
                    else
                    {
                        var symbols = ImportsDb.GetSymbolsWithOptionsAsync(cancellationToken)
                            .GetAwaiter().GetResult().ToArray();
                        return ImportOptionsAsync(symbols, cancellationToken);
                    }
                }
            }

            if (dataType == DataTypes.Fundamentals)
            {
                // find the symbols due for an update to their fundamentals (i.e., every 3 months).
                var meta = SymbolMetaDataRepository.Find(m => m.RequiresFundamentalUpdate).ToArray();

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
                    DomainEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
                        $"fundamentals for {exchange}", ApiService.Usage, ApiService.Usage), nameof(ImportFullAsync));
                }
            }

            return Task.CompletedTask;
        }

        private async Task ImportSplitsAsync(Symbol[] symbols, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var symbol in symbols)
            {
                var splits = (await DataClient.GetSplitsForSymbolAsync(symbol.Code, cancellationToken: cancellationToken)).ToList();

                List<Infrastructure.Domain.Split> domainSplits = new();

                splits.ForEach(s => domainSplits.Add(new Infrastructure.Domain.Split(symbol.Code, symbol.Exchange ?? "Unknown", s)));

                await ImportsDb.SaveSplitsAsync(domainSplits, cancellationToken);
            }
        }

        private async Task ImportDividendsAsync(Symbol[] symbols, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var symbol in symbols)
            {
                var divs = await DataClient.GetDividendsForSymbolAsync(symbol.Code, cancellationToken: cancellationToken);

                await ImportsDb.SaveDividendsAsync(symbol.Code, symbol.Exchange ?? "Unknown", divs, cancellationToken);
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
                    var prices = await DataClient.GetPricesForSymbolAsync(symbol.Code, cancellationToken: cancellationToken);
                    await ImportsDb.SavePriceActionsAsync(symbol.Code, symbol.Exchange ?? "Unknown", prices, cancellationToken);
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
                var options = await DataClient.GetOptionsForSymbolAsync(symbol.Code, cancellationToken: cancellationToken);

                await ImportsDb.SaveOptionsAsync(options, cancellationToken);
            }
        }

        private Task ImportBulkAsync(string exchange, string dataType, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (exchange == dataType && dataType == DataTypes.Exchanges)
            {
                var modelExchanges = DataClient.GetExchangeListAsync(cancellationToken).GetAwaiter().GetResult();

                return ImportsDb.SaveExchangesAsync(modelExchanges, cancellationToken);
            }

            return Task.CompletedTask;
        }

        private Task ImportFundamentalsAsync(string symbol, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fundamentals = DataClient.GetFundamentalsForSymbolAsync(symbol, cancellationToken: cancellationToken)
                .GetAwaiter().GetResult();

            if (fundamentals is EtfFundamentalsCollection etfCollection)
            {
                return ImportsDb.SaveEtfAsync(etfCollection, cancellationToken);
            }

            if (fundamentals is FundamentalsCollection collection)
            {
                return ImportsDb.SaveCompanyAsync(collection, cancellationToken);
            }

            throw new Exception($"Could not import fundamentals for {symbol}");
        }
    }
}
