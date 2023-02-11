using EodHistoricalData.Sdk;
using EodHistoricalData.Sdk.Events;
using EodHistoricalData.Sdk.Models;
using EodHistoricalData.Sdk.Models.Fundamentals.CommonStock;
using EodHistoricalData.Sdk.Models.Fundamentals.Etf;
using Import.AppServices.Configuration;
using Import.Infrastructure;
using Import.Infrastructure.Abstractions;
using Import.Infrastructure.Domain;
using Import.Infrastructure.Events;
using static Import.Infrastructure.Configuration.Constants;

namespace Import.AppServices;

public sealed class DataImportService
{
    private readonly ActionService actionService;

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

        actionService = new(ImportsDb);

        // TODO: try to make this better - maybe track when this is done and only do it every 24 hours.
        // EodHistoricalData.com doesn't automatically reset the usage counter until you make the first call of the day.
        // Since we have to make this call, we might as well preserve the results.
        var exchanges = DataClient.GetExchangeListAsync().GetAwaiter().GetResult();
        ImportsDb.SaveExchangesAsync(exchanges).GetAwaiter().GetResult();

        // Now our call to get available credits will always return a valid result.
        _ = DataClient.ResetUsageAsync(maxUsage).GetAwaiter().GetResult();
    }

    internal IDataClient DataClient { get; }

    internal ILogsDbContext LogsDb { get; }

    internal IImportsDbContext ImportsDb { get; }

    public async Task ResetMetaDataRepositoryAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        SymbolMetaDataRepository.SetItems(await ImportsDb.GetSymbolMetaDataAsync(cancellationToken));
    }

    public Task PurgeDataAsync(string purgeName,
        ImportCycle cycle,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (purgeName == PurgeName.Logs)
        {
            return LogsDb.PurgeLogsAsync(cancellationToken);
        }

        if (purgeName == PurgeName.Imports)
        {
            return ImportsDb.PurgeAsync(cancellationToken);
        }

        if (purgeName == PurgeName.Cycles)
        {
            var directoriesToRemove = cycle.OutputDirectory.Parent?.GetDirectories()
                .Where(d => !d.Name.Equals(cycle.Id)) ?? Enumerable.Empty<DirectoryInfo>();

            foreach (var dir in directoriesToRemove)
            {
                try
                {
                    dir.Delete(true);
                }
                catch
                {
                    DomainEventPublisher.RaiseMessageEvent(this, $"Could not delete {dir.FullName}", nameof(PurgeDataAsync), Microsoft.Extensions.Logging.LogLevel.Warning);
                }
            }
        }

        return Task.CompletedTask;
    }

    public async Task TruncateLogsAsync(string logLevel, DateTime date, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (date > DateTime.UtcNow) { await Task.CompletedTask; }

        await LogsDb.TruncateLogsAsync(logLevel, date, cancellationToken);
    }

    public Task ImportDataAsync(ActionItem action, ImportConfiguration importConfiguration, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (importConfiguration.Exchanges == null)
        {
            throw new ArgumentException($"{nameof(importConfiguration.Exchanges)} cannot be null when importing data.");
        }

        string scope = action.TargetScope ??
            throw new ArgumentException($"{nameof(action.TargetScope)} cannot be null when importing data.");

        string exchangeCode = action.ExchangeCode ??
            throw new ArgumentException($"{nameof(action.ExchangeCode)} cannot be null when importing data.");

        string dataType = action.TargetDataType ??
            throw new ArgumentException($"{nameof(action.TargetDataType)} cannot be null when importing data.");

        DomainEventPublisher.RaiseMessageEvent(this, $"Importing\t{scope} {exchangeCode} {dataType} {action.TargetName}".Trim(),
            nameof(ImportDataAsync),
            Microsoft.Extensions.Logging.LogLevel.Information);

        if (action.TargetDataType == DataTypes.Symbols)
        {
            var symbols = DataClient.GetSymbolListAsync(exchangeCode, cancellationToken)
                .GetAwaiter().GetResult()
                .ToArray();

            HashSet<Symbol> symbolSet = new(symbols.Where(s =>
                importConfiguration.Exchanges.ContainsKey(exchangeCode) &&
                importConfiguration.Exchanges[exchangeCode]["Symbol Type"].Contains(s.Type) &&
                importConfiguration.Exchanges[exchangeCode]["Exchanges"].Contains(s.Exchange) &&
                !SymbolsToIgnore.IsOnList(s.Code ?? "", s.Exchange ?? Constants.UnknownValue)));

            Task t = ImportsDb.SaveSymbolsAsync(symbolSet, exchangeCode, cancellationToken);

            foreach (var s in symbolSet)
            {
                SymbolMetaDataRepository.AddOrUpdate(new SymbolMetaData($"{s.Code!}.{exchangeCode}", s.Code!, s.Exchange, s.Type));
            }
            return t;

        }
        else if (scope == DataTypeScopes.Full)
        {
            return ImportFullAsync(action, importConfiguration, cancellationToken);
        }
        else if (scope == DataTypeScopes.Bulk)
        {
            return ImportBulkAsync(exchangeCode, dataType, cancellationToken);
        }

        return Task.CompletedTask;

    }

    public Task SaveSymbolsToIgnoreAsync(CancellationToken cancellationToken = default)
    {
        return ImportsDb.SaveSymbolsToIgnore(SymbolsToIgnore.GetAll(), cancellationToken);
    }

    public ImportCycle GetImportCycle(ImportConfiguration importConfiguration,
        DirectoryInfo outputDirectory)
    {
        var actions = actionService.GetSortedActionItems(importConfiguration);

        var result = new ImportCycle(outputDirectory);

        foreach (var action in actions)
        {
            if (!result.TryAddAction(action, out string? reason))
            {
                DomainEventPublisher.RaiseMessageEvent(this, $"Action ({action}) rejected because: {reason}", nameof(GetImportCycle));
            }
        }

        return result;
    }

    public static void CalculateCost(ImportCycle cycle)
    {
        // Calculate cost.
        int availableCredits = ApiService.Available;

        foreach (var action in cycle.Actions)
        {
            if (string.IsNullOrWhiteSpace(action.TargetDataType))
            {
                action.EstimatedCost = 0;
                continue;
            }

            var uri = FindImportUri(action.TargetDataType);

            int baseCost = ApiService.GetCost(uri);

            int symbolCount = SymbolMetaDataRepository.Find(s =>
                s.Exchange != null &&
                s.Exchange.Equals(action.TargetName, StringComparison.InvariantCultureIgnoreCase)).Count();

            if (symbolCount == 0 && action.ActionName == ActionNames.Import &&
                action.TargetDataType != DataTypes.Exchanges)
            {
                action.EstimatedCost = null;
            }
            else
            {
                int factor = action.TargetDataType! switch
                {
                    DataTypes.Dividends => symbolCount,
                    DataTypes.Exchanges => 1,
                    DataTypes.Fundamentals => SymbolMetaDataRepository.RequiresFundamentalsCount(action.TargetName),
                    DataTypes.Options => symbolCount,
                    DataTypes.Prices => symbolCount,
                    DataTypes.Splits => symbolCount,
                    DataTypes.Symbols => 1,
                    _ => 0
                };

                int cost = baseCost * factor;

                action.EstimatedCost = cost;

                availableCredits -= cost;
            }
        }
    }

    private static string? FindImportUri(string dataType)
    {
        return dataType switch
        {
            DataTypes.Options => ApiService.OptionsUri,
            DataTypes.Dividends => ApiService.DividendUri,
            DataTypes.Splits => ApiService.SplitsUri,
            DataTypes.Prices => ApiService.EodUri,
            DataTypes.Exchanges => ApiService.ExchangesUri,
            DataTypes.Fundamentals => ApiService.FundamentalsUri,
            DataTypes.Symbols => ApiService.ExchangeSymbolListUri,
            _ => null
        };
    }

    private Task ImportFullAsync(
        ActionItem action,
        ImportConfiguration importConfiguration,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (action.TargetDataType == DataTypes.Exchanges) // both Exchange and DateType are "Exchanges"
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

        var symbolsForExchange = SymbolMetaDataRepository.Find(action).ToArray();

        if (!symbolsForExchange?.Any() ?? false)
        {
            return Task.CompletedTask;
        }

        var exchange = action.TargetName;

        //ImportsDb.GetSymbolsForExchangeAsync(action.TargetName, cancellationToken);

        //var symbolsForExchange = allSymbols.Where(s => s.Exchange == exchange).Except(allSymbols.Where(s =>
        //    SymbolsToIgnore.IsOnList(s.Code ?? "", s.Exchange ?? Constants.UnknownValue))).ToArray();

        if (action.TargetDataType == DataTypes.Splits)
        {
            int estimatedCost = ApiService.GetCost(ApiService.SplitsUri, symbolsForExchange!.Length);
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

        if (action.TargetDataType == DataTypes.Dividends)
        {
            int estimatedCost = ApiService.GetCost(ApiService.DividendUri, symbolsForExchange!.Length);
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

        if (action.TargetDataType == DataTypes.Prices)
        {
            int estimatedCost = ApiService.GetCost(ApiService.EodUri, symbolsForExchange!.Length);
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

        if (action.TargetDataType == DataTypes.Options)
        {
            var symbolsWithOptions = SymbolMetaDataRepository.Find(s => s.LastUpdatedOptions is not null).ToArray();

            if (symbolsWithOptions.Any())
            {
                int estimatedCost = ApiService.GetCost(ApiService.OptionsUri, symbolsForExchange!.Length);
                if (estimatedCost > ApiService.Available)
                {
                    ApiEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
                        $"options for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage), nameof(ImportFullAsync));
                }
                else
                {
                    var symbols = ImportsDb.GetSymbolsWithOptionsAsync(cancellationToken)
                        .GetAwaiter().GetResult().ToArray();
                    return ImportOptionsAsync(symbolsWithOptions, cancellationToken);
                }
            }
        }

        if (action.TargetDataType == DataTypes.Fundamentals)
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

    private async Task ImportSplitsAsync(SymbolMetaData[] symbols, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var symbol in symbols)
        {
            if (symbol.Code != null)
            {
                var splits = (await DataClient.GetSplitsForSymbolAsync(symbol.Code, cancellationToken: cancellationToken)).ToList();

                List<Infrastructure.Domain.Split> domainSplits = new();

                splits.ForEach(s => domainSplits.Add(new Infrastructure.Domain.Split(symbol.Code, symbol.Exchange ?? Constants.UnknownValue, s)));

                await ImportsDb.SaveSplitsAsync(domainSplits, cancellationToken);
            }
        }
    }

    private async Task ImportDividendsAsync(SymbolMetaData[] symbols, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var symbol in symbols)
        {
            if (symbol.Code != null)
            {
                var divs = await DataClient.GetDividendsForSymbolAsync(symbol.Code, cancellationToken: cancellationToken);

                await ImportsDb.SaveDividendsAsync(symbol.Code, symbol.Exchange ?? Constants.UnknownValue,
                    divs!, cancellationToken);
            }
        }
    }

    private async Task ImportPricesAsync(SymbolMetaData[] symbols, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await Parallel.ForEachAsync(symbols, new ParallelOptions()
        {
            //MaxDegreeOfParallelism = 5,
            CancellationToken = cancellationToken
        }, async (symbol, t) =>
        {
            if (symbol.Code != null)
            {
                // Doing both the external API call and the save to the database inside this loop
                // to add some self-throttling to the bombarding of the API.
                var prices = await DataClient.GetPricesForSymbolAsync(symbol.Code, cancellationToken: cancellationToken);
                await ImportsDb.SavePriceActionsAsync(symbol.Code, symbol.Exchange ?? Constants.UnknownValue, prices, cancellationToken);
            }
        });
    }

    private async Task ImportOptionsAsync(SymbolMetaData[] symbols, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var symbol in symbols)
        {
            if (symbol.Code != null)
            {
                var options = await DataClient.GetOptionsForSymbolAsync(symbol.Code, cancellationToken: cancellationToken);

                await ImportsDb.SaveOptionsAsync(options, cancellationToken);
            }
        }
    }

    private Task ImportBulkAsync(string exchange, string dataType, CancellationToken cancellationToken = default)
    {
        // TODO: This function is lacking; needs some love. Bulk import has been neglected because its unreliable - not all tickers are included in the results.
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

        bool ignoreMissingFundamentals = importConfiguration.Options.ReasonsToIgnore != null &&
            importConfiguration.Options.ReasonsToIgnore.Contains(ImportConfiguration.ReasonToIgnoreValues.MissingFundamentals,
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
    }
}
