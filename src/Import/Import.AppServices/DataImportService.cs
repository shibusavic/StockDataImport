using EodHistoricalData.Sdk;
using EodHistoricalData.Sdk.Events;
using EodHistoricalData.Sdk.Models;
using EodHistoricalData.Sdk.Models.Fundamentals.CommonStock;
using EodHistoricalData.Sdk.Models.Fundamentals.Etf;
using Import.AppServices.Configuration;
using Import.Infrastructure;
using Import.Infrastructure.Abstractions;
using Import.Infrastructure.Events;
using static Import.Infrastructure.Configuration.Constants;

namespace Import.AppServices;

public sealed class DataImportService
{
    //private readonly ILogger? logger;
    private readonly HashSet<Symbol> allSymbols = new();

    internal DataImportService(ILogsDbContext logsDbContext,
        IImportsDbContext importsDbContext,
        string apiKey,
        int maxUsage = 100_000)
        : this(logsDbContext, importsDbContext, new DataClient(apiKey), maxUsage)
    { }

    internal DataImportService(ILogsDbContext logsDbContext,
        IImportsDbContext importsDbContext,
        IDataClient dataClient,
        int maxUsage = 100_000)
    {
        LogsDb = logsDbContext;
        ImportsDb = importsDbContext;

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
        ImportConfiguration importConfiguration,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        ApiEventPublisher.RaiseMessageEvent(this, $"Importing\t{scope} {exchange} {dataType}", nameof(ImportDataAsync),
            Microsoft.Extensions.Logging.LogLevel.Information);

        if (dataType == DataTypes.Symbols)
        {
            var symbols = DataClient.GetSymbolListAsync(exchange, cancellationToken)
                .GetAwaiter().GetResult()
                .ToArray();

            HashSet<Symbol> symbolSet = new(symbols.Where(s =>
                !SymbolsToIgnore.IsOnList(s.Code, s.Exchange ?? Constants.UnknownValue)));

            //var newSymbols = symbolSet.Except(allSymbols);
            //var missingSymbols = allSymbols.Except(symbolSet);
            // TODO: Do something with missing symbols here.

            allSymbols.UnionWith(symbolSet);

            return ImportsDb.SaveSymbolsAsync(symbolSet, exchange, cancellationToken);
        }
        else if (scope == DataTypeScopes.Full)
        {
            return ImportFullAsync(exchange, dataType, importConfiguration, cancellationToken);
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

    private Task ImportFullAsync(string exchange, string dataType,
        ImportConfiguration importConfiguration,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (exchange == dataType && dataType == DataTypes.Exchanges) // both Exchange and DateType are "Exchanges"
        {
            int estimatedCost = ApiService.GetCost(ApiService.ExchangesUri);
            if (estimatedCost > ApiService.Available)
            {
                ApiEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
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
            // TODO: not too sure about this line. I don't think we want to union here.
            allSymbols.UnionWith(ImportsDb.GetAllSymbolsAsync(cancellationToken).GetAwaiter().GetResult());
        }

        var symbolsForExchange = allSymbols.Where(s => s.Exchange == exchange).Except(allSymbols.Where(s =>
            SymbolsToIgnore.IsOnList(s.Code, s.Exchange ?? Constants.UnknownValue))).ToArray();

        if (dataType == DataTypes.Splits)
        {
            int estimatedCost = ApiService.GetCost(ApiService.SplitsUri, symbolsForExchange.Length);
            if (estimatedCost > ApiService.Available)
            {
                ApiEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
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
                ApiEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
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
                ApiEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
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
                    ApiEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
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
            var meta = SymbolMetaDataRepository.Find(m => m.RequiresFundamentalUpdate &&
                (m.Exchange?.Equals(exchange, StringComparison.InvariantCultureIgnoreCase) ?? false)).ToArray();

            DomainEventPublisher.RaiseMessageEvent(this, $"{meta.Length} missing fundamental records.",
                nameof(ImportFullAsync), Microsoft.Extensions.Logging.LogLevel.Information);

            int baseCost = ApiService.GetCost(ApiService.FundamentalsUri, 1);

            int estimatedCost = baseCost * meta.Length;

            int availableCycles = Math.Min(estimatedCost, ApiService.Available) / baseCost;

            if (meta.Any())
            {
                for (int i = 0; i < availableCycles; i++)
                {
                    ImportFundamentalsAsync(meta[i].Symbol, meta[i].Exchange ?? Constants.UnknownValue,
                        importConfiguration, cancellationToken)
                        .GetAwaiter().GetResult();

                    if (ApiService.Usage >= ApiService.DailyLimit)
                    {
                        break;
                    }
                }
            }

            if (ApiService.Usage >= ApiService.DailyLimit)
            {
                ApiEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
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

            splits.ForEach(s => domainSplits.Add(new Infrastructure.Domain.Split(symbol.Code, symbol.Exchange ?? Constants.UnknownValue, s)));

            await ImportsDb.SaveSplitsAsync(domainSplits, cancellationToken);
        }
    }

    private async Task ImportDividendsAsync(Symbol[] symbols, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var symbol in symbols)
        {
            var divs = await DataClient.GetDividendsForSymbolAsync(symbol.Code, cancellationToken: cancellationToken);

            await ImportsDb.SaveDividendsAsync(symbol.Code, symbol.Exchange ?? Constants.UnknownValue,
                divs!, cancellationToken);
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
            var prices = await DataClient.GetPricesForSymbolAsync(symbol.Code, cancellationToken: cancellationToken);
            await ImportsDb.SavePriceActionsAsync(symbol.Code, symbol.Exchange ?? Constants.UnknownValue, prices, cancellationToken);
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

    private Task ImportFundamentalsAsync(string symbol,
        string exchange,
        ImportConfiguration importConfiguration,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        bool ignoreMissingFundamentals = importConfiguration.ReasonsToIgnore != null &&
            importConfiguration.ReasonsToIgnore.Contains(ImportConfiguration.ReasonToIgnoreValues.MissingFundamentals,
                StringComparer.InvariantCultureIgnoreCase);

        var fundamentals = DataClient.GetFundamentalsForSymbolAsync(symbol, cancellationToken: cancellationToken)
            .GetAwaiter().GetResult();

        if (fundamentals == null)
        {
            if (ignoreMissingFundamentals)
            {
                SymbolsToIgnore.Add(new IgnoredSymbol(symbol, exchange, "Missing Fundamentals"));
            }
            return Task.CompletedTask;
        }

        bool saveFundamentals = true;

        if (fundamentals is EtfFundamentalsCollection etfCollection)
        {
            if (string.IsNullOrWhiteSpace(etfCollection.General.Name))
            {
                saveFundamentals = false;

                if (ignoreMissingFundamentals)
                {
                    SymbolsToIgnore.Add(new IgnoredSymbol(symbol, exchange, ImportConfiguration.ReasonToIgnoreValues.MissingFundamentals));
                }
            }

            if (saveFundamentals)
            {
                return ImportsDb.SaveEtfAsync(etfCollection, cancellationToken);
            }
        }

        if (fundamentals is FundamentalsCollection collection)
        {
            if (string.IsNullOrWhiteSpace(collection.General.Name))
            {
                saveFundamentals = false;

                if (ignoreMissingFundamentals)
                {
                    SymbolsToIgnore.Add(new IgnoredSymbol(symbol, exchange, ImportConfiguration.ReasonToIgnoreValues.MissingFundamentals));
                }
            }

            if (saveFundamentals)
            {
                return ImportsDb.SaveCompanyAsync(collection, cancellationToken);
            }
        }

        return Task.CompletedTask;
        //throw new Exception($"Could not import fundamentals for {symbol} {exchange}");
    }
}
