﻿using EodHistoricalData.Sdk;
using EodHistoricalData.Sdk.Events;
using EodHistoricalData.Sdk.Models;
using EodHistoricalData.Sdk.Models.Bulk;
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
        // Since we have to make a call, it might as well be this call, and we might as well preserve the results.
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
        SymbolMetaDataRepository.SetItems(await ImportsDb.CreateSymbolMetaDataAsync(cancellationToken));
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

        string mode = action.Mode;

        string scope = action.TargetScope ??
            throw new ArgumentException($"{nameof(action.TargetScope)} cannot be null when importing data.");

        string exchangeCode = action.ExchangeCode ??
            throw new ArgumentException($"{nameof(action.ExchangeCode)} cannot be null when importing data.");

        string dataType = action.TargetDataType ??
            throw new ArgumentException($"{nameof(action.TargetDataType)} cannot be null when importing data.");

        DomainEventPublisher.RaiseMessageEvent(this, $"Importing {mode} {scope} {exchangeCode} {dataType} {action.TargetName}".Trim(),
            nameof(ImportDataAsync),
            Microsoft.Extensions.Logging.LogLevel.Information);

        if (action.TargetDataType == DataTypes.Symbols)
        {
            var symbols = DataClient.GetSymbolListAsync(exchangeCode, cancellationToken)
                .GetAwaiter().GetResult()
                .ToArray();

            Symbol[] symbolsToSave = symbols.Where(s =>
                importConfiguration.Exchanges.ContainsKey(exchangeCode) &&
                importConfiguration.Exchanges[exchangeCode]["Symbol Type"].Contains(s.Type) &&
                importConfiguration.Exchanges[exchangeCode]["Exchanges"].Contains(s.Exchange) &&
                !SymbolsToIgnore.IsOnList(s.Code ?? "", s.Exchange ?? Constants.UnknownValue)).ToArray();

            Task t = ImportsDb.SaveSymbolsAsync(symbolsToSave, exchangeCode, cancellationToken);

            foreach (var s in symbolsToSave)
            {
                SymbolMetaDataRepository.AddOrUpdate(new SymbolMetaData($"{s.Code!}.{exchangeCode}", s.Code!, s.Exchange, s.Type, s.Name));
            }

            return t;
        }
        else if (scope == DataTypeScopes.Full)
        {
            return ImportFullAsync(action, importConfiguration, cancellationToken);
        }
        else if (scope == DataTypeScopes.Bulk)
        {
            return ImportBulkAsync(action, importConfiguration, cancellationToken);
        }
        else if (scope == DataTypeScopes.TryBulkThenFull)
        {
            return ImportBulkThenFullAsync(action, importConfiguration, cancellationToken);
        }

        return Task.CompletedTask;
    }

    public Task SaveSymbolsToIgnoreAsync(CancellationToken cancellationToken = default)=>
        ImportsDb.SaveSymbolsToIgnore(SymbolsToIgnore.GetAll(), cancellationToken);

    public async Task SaveMetaDataAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await ImportsDb.SaveSymbolMetaDataAsync(
            await ImportsDb.CreateSymbolMetaDataAsync(cancellationToken), cancellationToken);
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

    public static void CalculateCost(ImportCycle cycle, ImportConfiguration importConfiguration)
    {
        int availableCredits = ApiService.Available;

        var exchanges = importConfiguration.GetExchanges();

        List<string> subExchanges = new();

        foreach (var exchange in exchanges)
        {
            subExchanges.AddRange(importConfiguration.GetSubExchanges(exchange));
        }

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
                s.Exchange.Equals(action.TargetName, StringComparison.OrdinalIgnoreCase)).Count();

            int factor = 0;

            if (action.Mode == Modes.Economy)
            {
                factor = action.TargetDataType! switch
                {
                    DataTypes.Exchanges => 1,
                    DataTypes.Dividends => SymbolMetaDataRepository.GetAll().Count(m => m.HasDividends),
                    DataTypes.Splits => SymbolMetaDataRepository.GetAll().Count(m => m.HasSplits),
                    DataTypes.Prices => symbolCount,
                    DataTypes.Options => SymbolMetaDataRepository.GetAll().Count(m => m.HasOptions),
                    DataTypes.Fundamentals => SymbolMetaDataRepository.RequiresFundamentalsCount(action.TargetName),
                    DataTypes.Trends => SymbolMetaDataRepository.Count(s => s.Exchange != null &&
                        subExchanges.Contains(s.Exchange)),
                    DataTypes.Exchanges
                        or DataTypes.Earnings
                        or DataTypes.Ipos => 1,
                    _ => 0
                };
            }
            else
            {
                factor = action.TargetDataType! switch
                {
                    DataTypes.Exchanges => 1,
                    DataTypes.Dividends
                        or DataTypes.Options
                        or DataTypes.Prices
                        or DataTypes.Splits => symbolCount,
                    DataTypes.Fundamentals => symbolCount,
                    DataTypes.Trends => SymbolMetaDataRepository.Count(s => s.Exchange != null &&
                        subExchanges.Contains(s.Exchange)),
                    DataTypes.Exchanges
                        or DataTypes.Earnings
                        or DataTypes.Ipos => 1,
                    _ => 0
                };
            }

            int cost = baseCost * factor;

            action.EstimatedCost = cost;

            availableCredits -= cost;
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
            DataTypes.Ipos => ApiService.CalendarUri,
            DataTypes.Trends => ApiService.CalendarUri,
            DataTypes.Earnings => ApiService.CalendarUri,
            _ => null
        };
    }

    private Task ImportFullAsync(
        ActionItem action,
        ImportConfiguration importConfiguration,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (action.TargetScope != DataTypeScopes.Full)
        {
            throw new ArgumentException($"Action with scope {action.TargetScope ?? Constants.UnknownValue} sent to {nameof(ImportFullAsync)}");
        }

        if (action.TargetDataType == DataTypes.Exchanges) // both Exchange and DataType are "Exchanges"
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

        SymbolMetaData[] symbolsForExchange = SymbolMetaDataRepository.Find(action).ToArray();

        var exchange = action.TargetName;

        if (action.TargetDataType == DataTypes.Splits)
        {
            if (action.Mode == Modes.Economy)
            {
                symbolsForExchange = symbolsForExchange!.Where(s => s.HasSplits).ToArray();
            }

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
            if (action.Mode == Modes.Economy)
            {
                symbolsForExchange = symbolsForExchange!.Where(s => s.HasDividends).ToArray();
            }

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
            if (action.Mode == Modes.Economy)
            {
                symbolsForExchange = symbolsForExchange!.Where(s => s.HasOptions).ToArray();
            }

            int estimatedCost = ApiService.GetCost(ApiService.OptionsUri, symbolsForExchange!.Length);
            if (estimatedCost > ApiService.Available)
            {
                ApiEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
                    $"options for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage), nameof(ImportFullAsync));
            }
            else
            {
                return ImportOptionsAsync(symbolsForExchange, cancellationToken);
            }
        }

        if (action.TargetDataType == DataTypes.Fundamentals)
        {
            // find the symbols due for an update to their fundamentals (i.e., every 3 months).
            var meta = SymbolMetaDataRepository.Find(m => m.RequiresFundamentalUpdate &&
                (m.Exchange?.Equals(exchange, StringComparison.OrdinalIgnoreCase) ?? false)).ToArray();

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

        if (action.TargetDataType == DataTypes.Ipos)
        {
            int estimatedCost = ApiService.GetCost(ApiService.CalendarUri, 1);
            if (estimatedCost > ApiService.Available)
            {
                ApiEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
                    $"IPOs for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage), nameof(ImportFullAsync));
            }
            else
            {
                var subExchanges = importConfiguration.GetSubExchanges(action.TargetName);

                if (subExchanges.Any())
                {
                    return ImportIposAsync(subExchanges, cancellationToken);
                }
            }
        }

        if (action.TargetDataType == DataTypes.Earnings)
        {
            int estimatedCost = ApiService.GetCost(ApiService.CalendarUri, 1);
            if (estimatedCost > ApiService.Available)
            {
                ApiEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
                    $"Earnings for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage), nameof(ImportFullAsync));
            }
            else
            {
                var subExchanges = importConfiguration.GetSubExchanges(action.TargetName);

                if (subExchanges.Any())
                {
                    return ImportEarningsAsync(subExchanges, cancellationToken);
                }
            }
        }

        if (action.TargetDataType == DataTypes.Trends)
        {
            var subExchanges = importConfiguration.GetSubExchanges(action.TargetName);

            if (subExchanges.Any())
            {
                int estimatedCost = ApiService.GetCost(ApiService.CalendarUri,
                    SymbolMetaDataRepository.Count(s => subExchanges.Contains(s.Exchange)));

                if (estimatedCost > ApiService.Available)
                {
                    ApiEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
                        $"Trends for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage), nameof(ImportFullAsync));
                }
                else
                {
                    return ImportTrendsAsync(subExchanges, cancellationToken);
                }
            }
        }

        return Task.CompletedTask;
    }

    private Task ImportBulkAsync(
        ActionItem action,
        ImportConfiguration importConfiguration,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (action.TargetScope != DataTypeScopes.Bulk && action.TargetScope != DataTypeScopes.TryBulkThenFull)
        {
            throw new ArgumentException($"Action with scope {action.TargetScope ?? Constants.UnknownValue} sent to {nameof(ImportBulkAsync)}");
        }

        var symbolsForExchange = SymbolMetaDataRepository.Find(action).ToArray();

        var exchange = action.TargetName;

        if (action.ExchangeCode == null) throw new ArgumentException($"Exchange Code cannot be null on bulk import actions");

        if (action.TargetDataType == DataTypes.Splits)
        {
            int estimatedCost = ApiService.GetCost(ApiService.BulkEodUri, 1);

            if (estimatedCost > ApiService.Available)
            {
                ApiEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
                    $"bulk splits for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage), nameof(ImportBulkAsync));
            }
            else
            {
                return ImportBulkSplitsAsync(action.ExchangeCode, null, cancellationToken);
            }
        }

        if (action.TargetDataType == DataTypes.Dividends)
        {
            int estimatedCost = ApiService.GetCost(ApiService.DividendUri, 1);
            if (estimatedCost > ApiService.Available)
            {
                ApiEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
                    $"bulk dividends for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage), nameof(ImportBulkAsync));
            }
            else
            {
                return ImportBulkDividendsAsync(action.ExchangeCode, null, cancellationToken);
            }
        }

        if (action.TargetDataType == DataTypes.Prices)
        {
            int estimatedCost = ApiService.GetCost(ApiService.EodUri, 1);
            if (estimatedCost > ApiService.Available)
            {
                ApiEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
                    $"bulk prices for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage), nameof(ImportBulkAsync));
            }
            else
            {
                return ImportBulkPriceActionsAsync(action.ExchangeCode, null, cancellationToken);
            }
        }

        return Task.CompletedTask;
    }

    private Task ImportBulkThenFullAsync(
        ActionItem action,
        ImportConfiguration importConfiguration,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (action.TargetScope != DataTypeScopes.TryBulkThenFull)
        {
            throw new ArgumentException($"Action with scope {action.TargetScope ?? Constants.UnknownValue} sent to {nameof(ImportBulkThenFullAsync)}");
        }

        if (action.TargetDataType is not null &&
            action.TargetDataType is DataTypes.Splits or DataTypes.Dividends)
        {
            return ImportBulkAsync(action, importConfiguration, cancellationToken);
        }

        var symbolsForExchange = SymbolMetaDataRepository.Find(action).ToArray();

        var exchange = action.TargetName;

        if (action.ExchangeCode == null) throw new ArgumentException($"Exchange Code cannot be null on bulk import actions");

        if (action.TargetDataType == DataTypes.Prices)
        {
            int estimatedCost = ApiService.GetCost(ApiService.EodUri, 1);
            if (estimatedCost > ApiService.Available)
            {
                ApiEventPublisher.RaiseApiLimitReachedEvent(this, new ApiLimitReachedException(
                    $"bulk prices for {exchange}", ApiService.Usage, estimatedCost + ApiService.Usage), nameof(ImportBulkThenFullAsync));
            }
            else
            {
                var beforeCount = SymbolMetaDataRepository.Count();
                var beforeLastUpdate = beforeCount == 0 ? DateTime.MinValue : SymbolMetaDataRepository.GetAll().Select(s => s.LastUpdated).Max();

                ImportBulkPriceActionsAsync(action.ExchangeCode, null, cancellationToken)
                    .GetAwaiter().GetResult();

                var unchanged = SymbolMetaDataRepository.Find(s => s.LastUpdated < beforeLastUpdate).ToArray();

                if (unchanged.Any())
                {
                    DomainEventPublisher.RaiseMessageEvent(this,
                        $"{unchanged.Length} symbols not updated in bulk import.",
                        nameof(ImportBulkThenFullAsync), Microsoft.Extensions.Logging.LogLevel.Information);

                    return ImportPricesAsync(unchanged, cancellationToken);
                }
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

                SymbolMetaDataRepository.Get(symbol.Code)?.Update();
            }
        }
    }

    private async Task ImportBulkSplitsAsync(string exchangeCode, DateOnly? date = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var bulkSplits = (await DataClient.GetBulkSplitsForExchangeAsync(exchangeCode, date, cancellationToken)).ToArray();

        List<Infrastructure.Domain.Split> domainSplits = new();

        foreach (var bs in bulkSplits)
        {
            string code = $"{bs.Code}.{bs.Exchange}";

            var existing = SymbolMetaDataRepository.Find(s => s.Code == code).FirstOrDefault();

            if (existing?.Exchange != null)
            {
                existing.Update();
                domainSplits.Add(new Infrastructure.Domain.Split(bs, existing.Exchange));
            }
        }

        var t = ImportsDb.SaveSplitsAsync(domainSplits, cancellationToken);

        foreach (var ds in domainSplits.Select(d => (d.Symbol, d.Exchange)).Distinct())
        {
            SymbolMetaDataRepository.Get(ds.Symbol, ds.Exchange)?.Update();
        }

        await t;
    }

    private async Task ImportDividendsAsync(SymbolMetaData[] symbols, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var symbol in symbols)
        {
            if (symbol.Code != null)
            {
                var divs = await DataClient.GetDividendsForSymbolAsync(symbol.Code, cancellationToken: cancellationToken);

                if (divs.Any())
                {
                    await ImportsDb.SaveDividendsAsync(symbol.Code, symbol.Exchange ?? Constants.UnknownValue,
                        divs!, cancellationToken);

                    SymbolMetaDataRepository.Get(symbol.Code)?.Update();
                }
            }
        }
    }

    private async Task ImportBulkDividendsAsync(string exchangeCode,
        DateOnly? date = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var bulkDividends = (await DataClient.GetBulkDividendsForExchangeAsync(exchangeCode, date, cancellationToken)).ToArray();

        List<(BulkDividend BulkDividend, string Exchange)> dividends = new();

        foreach (var bd in bulkDividends)
        {
            var existing = SymbolMetaDataRepository.Get($"{bd.Code}.{bd.Exchange}"); // e.g., Exchange = "US"

            if (existing?.Exchange != null)
            {
                dividends.Add((bd, existing.Exchange)); // e.g., existing.Exchange = "NYSE"
            }
        }

        var exchanges = dividends.Select(d => d.Exchange).Distinct();

        foreach (var exchange in exchanges)
        {
            var divsForExchange = dividends.Where(d => d.Exchange == exchange).Select(d => d.BulkDividend);

            if (divsForExchange?.Any() ?? false)
            {
                var t = ImportsDb.SaveBulkDividendsAsync(divsForExchange, exchange, cancellationToken);

                var metaToUpdate = divsForExchange.Select(d => (d.Code, d.Exchange)).Distinct();

                foreach (var d in metaToUpdate)
                {
                    SymbolMetaDataRepository.Get($"{d.Code}.{d.Exchange}")?.Update();
                }

                await t;
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
                SymbolMetaDataRepository.Get(symbol.Code)?.Update();
            }
        });
    }

    private async Task ImportBulkPriceActionsAsync(string exchangeCode,
        DateOnly? date = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var bulkPrices = (await DataClient.GetBulkHistoricalDataForExchangeAsync(exchangeCode, null, date, cancellationToken)).ToArray();

        List<(BulkPriceAction PriceAction, string Exchange)> priceActions = new();

        foreach (var bp in bulkPrices)
        {
            var existing = SymbolMetaDataRepository.Find(s => s.Code == $"{bp.Code}.{bp.ExchangeShortName}").FirstOrDefault();

            if (existing?.Exchange != null)
            {
                priceActions.Add((bp, existing.Exchange));
            }
        }

        if (priceActions.Any())
        {
            var exchanges = priceActions.Select(p => p.Exchange).Distinct();

            foreach (var exchange in exchanges)
            {
                var pricesForExchange = priceActions.Where(p => p.Exchange == exchange)
                    .Select(s => s.PriceAction);

                var t = ImportsDb.SaveBulkPriceActionsAsync(pricesForExchange, exchange, cancellationToken);

                var metaUpdates = pricesForExchange.Select(p => (p.Code, p.ExchangeShortName)).Distinct();

                foreach (var (Code, ExchangeShortName) in metaUpdates)
                {
                    SymbolMetaDataRepository.Get($"{Code}.{ExchangeShortName}")?.Update();
                }

                await t;
            }
        }
    }

    private async Task ImportOptionsAsync(SymbolMetaData[] symbols, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var symbol in symbols)
        {
            if (symbol.Code != null)
            {
                var options = await DataClient.GetOptionsForSymbolAsync(symbol.Code, cancellationToken: cancellationToken);

                if (options.Data?.Any() ?? false)
                {
                    await ImportsDb.SaveOptionsAsync(options, cancellationToken);

                    SymbolMetaDataRepository.Get(symbol.Code)?.Update();
                }
            }
        }
    }

    private Task ImportFundamentalsAsync(string symbol,
        string exchange,
        ImportConfiguration importConfiguration,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        bool ignoreMissingFundamentals = importConfiguration.Options.ReasonsToIgnore != null &&
            importConfiguration.Options.ReasonsToIgnore.Contains(ImportConfiguration.ReasonToIgnoreValues.MissingFundamentals,
                StringComparer.OrdinalIgnoreCase);

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
                SymbolMetaDataRepository.Get(symbol, exchange)?.Update();
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
                SymbolMetaDataRepository.Get(symbol, exchange)?.Update();
                return ImportsDb.SaveCompanyAsync(collection, cancellationToken);
            }
        }

        return Task.CompletedTask;
    }

    private async Task ImportIposAsync(string[] exchanges, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (exchanges.Length == 0) { throw new ArgumentException($"{nameof(exchanges)} must have at least 1 exchange"); }

        var ipos = await DataClient.GetIposAsync(cancellationToken: cancellationToken);

        if (ipos.Ipos?.Any() ?? false)
        {
            var t = ImportsDb.SaveIposAsync(ipos, exchanges, cancellationToken);

            foreach (var ipo in ipos.Ipos.Where(i => i.Code != null))
            {
                SymbolMetaDataRepository.Get(ipo.Code!)?.Update();
            }

            await t;
        }
    }

    private async Task ImportEarningsAsync(string[] exchanges, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (exchanges.Length == 0) { throw new ArgumentException($"{nameof(exchanges)} must have at least 1 exchange"); }

        var earnings = await DataClient.GetEarningsAsync(cancellationToken: cancellationToken);

        if (earnings.Earnings?.Any() ?? false)
        {
            var t = ImportsDb.SaveEarningsAsync(earnings, exchanges, cancellationToken);

            foreach (var e in earnings.Earnings.Where(i => i.Code != null))
            {
                SymbolMetaDataRepository.Get(e.Code!)?.Update();
            }

            await t;
        }
    }

    private async Task ImportTrendsAsync(string[] exchanges, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (exchanges.Length == 0) { throw new ArgumentException($"{nameof(exchanges)} must have at least 1 exchange"); }

        var symbols = SymbolMetaDataRepository.Find(s => exchanges.Contains(s.Exchange));

        foreach (var chunk in symbols.Chunk(100))
        {
            var trends = await DataClient.GetTrendsForSymbolsAsync(
                string.Join(',', chunk.Select(c => c.Code)), cancellationToken);

            if (trends.Trends?.Any() ?? false)
            {
                var t = ImportsDb.SaveTrendsAsync(trends.Trends, exchanges, cancellationToken);

                foreach (var e in trends.Trends.Where(i => i.Code != null))
                {
                    SymbolMetaDataRepository.Get(e.Code!)?.Update();
                }

                await t;
            }
        }


    }
}
