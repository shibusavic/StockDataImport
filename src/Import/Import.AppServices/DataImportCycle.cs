using EodHistoricalData.Sdk;
using EodHistoricalData.Sdk.Events;
using Import.AppServices.Configuration;
using Import.Infrastructure;
using Import.Infrastructure.Abstractions;
using Import.Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Shibusa.Extensions;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using static Import.Infrastructure.Configuration.Constants;

namespace Import.AppServices;

public class DataImportCycle
{
    private readonly DataImportService dataImportService;
    private readonly ILogger? logger;
    private readonly Regex textToTimeRegex = new(@"(\d+)\s+(year|month|week|day)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

    internal DataImportCycle(DataImportService dataImportService, ILogger? logger = null)
    {

        Actions = Array.Empty<ActionItem>();
        this.dataImportService = dataImportService;
        this.logger = logger;
    }

    public ActionItem[] Actions { get; private set; }

    public async Task ExecuteAsync(ImportConfiguration importConfiguration,
        bool dryRun = false,
        CancellationToken cancellationToken = default)
    {
        Actions = (await GetSortedActionItemsAsync(importConfiguration)).ToArray();

        if (Actions.Any())
        {
            if (!dryRun)
            {
                DomainEventPublisher.RaiseMessageEvent(this, "Import started", nameof(ExecuteAsync));

                var saveTask = dataImportService.LogsDb.SaveActionItemsAsync(Actions, cancellationToken);

                DomainEventPublisher.RaiseMessageEvent(this, ApiService.GetAvailableCreditFormula(), nameof(DataImportCycle));

                DomainEventPublisher.RaiseMessageEvent(this, $"{Actions.Length} action(s) identified.", nameof(DataImportCycle));

                var purgeActions = Actions.Where(a => a.ActionName.Equals(ActionNames.Purge)).ToArray();

                var otherActions = Actions.Except(purgeActions).ToArray();

                await saveTask;

                if (purgeActions.Any())
                {
                    Actions = purgeActions;
                    await ExecuteAsync(cancellationToken);
                    await dataImportService.LogsDb.SaveActionItemsAsync(Actions, cancellationToken);
                    Actions = otherActions;
                    await dataImportService.DataClient.ResetUsageAsync(importConfiguration.MaxTokenUsage ?? 100000);
                }
            }

            // Calculate cost.
            int availableCredits = ApiService.Available;

            List<ActionItem> removalCandidates = new();

            foreach (var action in Actions)
            {
                if (!string.IsNullOrWhiteSpace(action.TargetDataType))
                {
                    var uri = FindImportUri(action.TargetDataType);

                    int baseCost = ApiService.GetCost(uri);

                    int factor = action.TargetDataType! switch
                    {
                        DataTypes.Dividends => SymbolMetaDataRepository.Count,
                        DataTypes.Exchanges => 1,
                        DataTypes.Fundamentals => SymbolMetaDataRepository.RequiresFundamentalsCount,
                        DataTypes.Options => SymbolMetaDataRepository.OptionsCount,
                        DataTypes.Prices => SymbolMetaDataRepository.Count,
                        DataTypes.Splits => SymbolMetaDataRepository.Count,
                        DataTypes.Symbols => SymbolMetaDataRepository.Count,
                        _ => 0
                    };

                    int cost = baseCost * factor;

                    action.EstimatedCost = cost;

                    if (cost > availableCredits)
                    {
                        removalCandidates.Add(action);
                    }
                    else
                    {
                        availableCredits -= cost;
                    }
                }
            }

            // All the specified actions were saved, but only those we can afford remain in this cycle.
            Actions = Actions.Except(removalCandidates).ToArray();

            if (!dryRun)
            {
                await ExecuteAsync(cancellationToken);
                await dataImportService.LogsDb.SaveActionItemsAsync(Actions, cancellationToken);
            }
        }
    }

    private async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        foreach (var action in Actions)
        {
            Stopwatch actionTimer = Stopwatch.StartNew();

            action.Start();

            DomainEventPublisher.RaiseMessageEvent(this,
                $"Running {action}",
                nameof(ExecuteAsync));

            try
            {
                if (action.ActionName == ActionNames.Skip)
                {
                    action.Complete();
                    continue;
                }

                if (action.ActionName == ActionNames.Fix)
                {
                    await dataImportService.ApplyFixAsync(action.TargetName, cancellationToken);
                }

                if (action.ActionName == ActionNames.Purge)
                {
                    await dataImportService.PurgeDataAsync(action.TargetName!, cancellationToken);
                }

                if (action.ActionName == ActionNames.Import)
                {
                    await dataImportService.ImportDataAsync(action.TargetScope!, action.TargetName!,
                        action.TargetDataType!,
                        cancellationToken);
                }

                if (action.ActionName == ActionNames.Truncate)
                {
                    if (DateTime.TryParse(action.TargetScope, out DateTime date))
                    {
                        if (action.TargetName == PurgeName.ApiResponses)
                        {
                            await dataImportService.TruncateApiResponsesAsync(date, cancellationToken);
                        }
                        else if (action.TargetName == PurgeName.ActionItems)
                        {
                            await dataImportService.TruncateActionItemsAsync(date, cancellationToken);
                        }
                        else
                        {
                            await dataImportService.TruncateLogsAsync(action.TargetName, date, cancellationToken);
                        }
                    }
                    else
                    {
                        throw new Exception($"Could not parse '{action.TargetScope}' as a DateTime.");
                    }
                }
            }
            catch (Exception exc)
            {
                action.Error(exc);
            }
            finally
            {
                if (action.Status == ImportActionStatus.InProgress)
                {
                    action.Complete();
                }

                actionTimer.Stop();
            }
        }
    }

    private async Task<IEnumerable<ActionItem>> GetSortedActionItemsAsync(ImportConfiguration config)
    {
        List<ActionItem> items = new();

        var exchanges = config.Exchanges;

        // get the undone items from the db.
        items.AddRange(await dataImportService.LogsDb.GetActionItemsByStatusAsync(ImportActionStatus.UsageRequirementMet
            | ImportActionStatus.Error | ImportActionStatus.InProgress | ImportActionStatus.NotStarted));

        if (config.Purges?.Any() ?? false)
        {
            foreach (var name in config.Purges)
            {
                items.Add(new ActionItem(ActionNames.Purge, name, null, null, 0));
            }
        }

        if (config.Fixes?.Any() ?? false)
        {
            foreach (var name in config.Fixes)
            {
                items.Add(new ActionItem(ActionNames.Fix, name, null, null, 100)); // send to the end of the list
            }
        }

        if (config.DataRetention?.Any() ?? false)
        {
            items.AddRange(GetDataRentionActions(config.DataRetention));
        }

        if (exchanges != null)
        {
            if (await dataImportService.ImportsDb.IsDatabaseEmptyAsync() && (config.OnEmptyDatabase?.Any() ?? false))
            {
                items.AddRange(ParseActionItems(nameof(config.OnEmptyDatabase), config.OnEmptyDatabase!));
            }

            if (config.AnyDay?.Any() ?? false)
            {
                items.AddRange(ParseActionItems(nameof(config.AnyDay), config.AnyDay!));
            }

            // purposefully choosing DateTime.Now over DateTime.UtcNow so that the day of week
            // is relative to the machine on which the code is executing.
            var dayOfWeek = DateTime.Now.DayOfWeek;

            if ((config.Sunday?.Any() ?? false) && dayOfWeek == DayOfWeek.Sunday)
            {
                items.AddRange(ParseActionItems(nameof(config.Sunday), config.Sunday!));
            }

            if ((config.Monday?.Any() ?? false) && dayOfWeek == DayOfWeek.Monday)
            {
                items.AddRange(ParseActionItems(nameof(config.Monday), config.Monday!));
            }

            if ((config.Tuesday?.Any() ?? false) && dayOfWeek == DayOfWeek.Tuesday)
            {
                items.AddRange(ParseActionItems(nameof(config.Tuesday), config.Tuesday!));
            }

            if ((config.Wednesday?.Any() ?? false) && dayOfWeek == DayOfWeek.Wednesday)
            {
                items.AddRange(ParseActionItems(nameof(config.Wednesday), config.Wednesday!));
            }

            if ((config.Thursday?.Any() ?? false) && dayOfWeek == DayOfWeek.Thursday)
            {
                items.AddRange(ParseActionItems(nameof(config.Thursday), config.Thursday!));
            }

            if ((config.Friday?.Any() ?? false) && dayOfWeek == DayOfWeek.Friday)
            {
                items.AddRange(ParseActionItems(nameof(config.Friday), config.Friday!));
            }

            if ((config.Saturday?.Any() ?? false) && dayOfWeek == DayOfWeek.Saturday)
            {
                items.AddRange(ParseActionItems(nameof(config.Saturday), config.Saturday!));
            }
        }

        return SortActionItems(items);
    }

    private static IEnumerable<ActionItem> ParseActionItems(string parent, ImportActions[]? actions)
    {
        if (actions?.Any() ?? false)
        {
            foreach (var action in actions)
            {
                if (action.Skip.GetValueOrDefault())
                {
                    yield return new ActionItem(actionName: ActionNames.Skip, targetName: parent);
                    continue;
                }

                if (action.DataTypes?.Contains(DataTypes.Exchanges) ?? false)
                {
                    yield return new ActionItem(actionName: ActionNames.Import,
                        targetName: DataTypes.Exchanges,
                        targetScope: DataTypeScopes.Full.ToString(),
                        targetDataType: DataTypes.Exchanges,
                        priority: action.Priority);
                }

                if (action.IsValidForImport())
                {
                    //foreach (var exchange in action.Exchanges!)
                    //{
                    //    if (exchange.Name != null)
                    //    {
                    //        foreach (var dataType in action.DataTypes!)
                    //        {
                    //            if (dataType == DataTypes.Exchanges) continue;
                    //            yield return new ActionItem(actionName: ActionNames.Import,
                    //                targetName: exchange.Name,
                    //                targetScope: action.Scope,
                    //                targetDataType: dataType,
                    //                priority: action.Priority);
                    //        }
                    //    }
                    //}
                }
            }
        }
    }

    private IEnumerable<ActionItem> GetDataRentionActions(IDictionary<string, string> dataRetention)
    {
        foreach (var kvp in dataRetention)
        {
            if (Enum.TryParse(kvp.Key, out LogLevel logLevel))
            {
                yield return new ActionItem(ActionNames.Truncate, logLevel.GetDescription(),
                    ConvertTextToDateTime(kvp.Value).ToString(), null, 5);
            }
            else if (kvp.Key == PurgeName.ApiResponses)
            {
                yield return new ActionItem(ActionNames.Truncate, kvp.Key,
                    ConvertTextToDateTime(kvp.Value).ToString(), null, 5);
            }
        }
    }

    private DateTime ConvertTextToDateTime(string text)
    {
        var matches = textToTimeRegex.Matches(text.ToLower());

        if (matches.Any())
        {
            if (int.TryParse(matches[0].Groups[1].Value, out int num))
            {
                num = -1 * Math.Abs(num);

                return matches[0].Groups[2].Value switch
                {
                    "year" => DateTime.Now.AddYears(num),
                    "month" => DateTime.Now.AddMonths(num),
                    "week" => DateTime.Now.AddDays(num * 7),
                    "day" => DateTime.Now.AddDays(num),
                    _ => DateTime.MinValue,
                };
            }
        }

        return DateTime.MinValue;
    }

    private static IEnumerable<ActionItem> SortActionItems(IEnumerable<ActionItem> items)
    {
        List<ActionItem> results = new();

        var actions = new List<ActionItem>(items);

        if (!actions.Any()) { return Enumerable.Empty<ActionItem>(); }

        if (actions.Any(a => a.ActionName == ActionNames.Skip))
        {
            actions.RemoveAll(r => r.ActionName == ActionNames.Import);
        }

        actions.Sort(ActionComparison);

        List<(string Action, string Target, string TargetDataType)> rollingList = new();

        foreach (var action in actions)
        {
            if (action.TargetName == null)
            {
                results.Add(action);
            }
            else
            {
                if (!rollingList.Contains((action.ActionName, action.TargetName, action.TargetDataType!)))
                {
                    rollingList.Add((action.ActionName, action.TargetName, action.TargetDataType!));
                    results.Add(action);
                }
            }
        }

        var fullImports = results.Where(r => r.ActionName == ActionNames.Import &&
            r.TargetScope == DataTypeScopes.Full);

        foreach (var fullImport in fullImports)
        {
            results.RemoveAll(r => r.ActionName == ActionNames.Import &&
                r.TargetScope == DataTypeScopes.Bulk &&
                r.TargetDataType == fullImport.TargetDataType);
        }

        return results;
    }

    private static int ActionComparison(ActionItem item1, ActionItem item2)
    {
        if (ReferenceEquals(item1, item2)) return 0;

        int result = item1.Priority.CompareTo(item2.Priority);

        if (result == 0) { result = item1.TargetScopeValue.GetValueOrDefault().CompareTo(item2.TargetScopeValue.GetValueOrDefault()); }
        if (result == 0) { result = item1.TargetDataTypeSortValue.GetValueOrDefault().CompareTo(item2.TargetDataTypeSortValue.GetValueOrDefault()); }

        return result;
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
}
