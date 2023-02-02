using Import.AppServices.Configuration;
using Import.Infrastructure.Abstractions;
using Import.Infrastructure.Domain;
using Microsoft.Extensions.Logging;
using Shibusa.Extensions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static Import.Infrastructure.Configuration.Constants;

[assembly: InternalsVisibleTo("Import.AppServices.Tests")]
namespace Import.AppServices;

public class ActionService
{
    private readonly ILogsDbContext logsDb;
    private readonly IImportsDbContext importsDb;

    internal ActionService(ILogsDbContext logsDbContext, IImportsDbContext importsDbContext)
    {
        logsDb = logsDbContext;
        importsDb = importsDbContext;
    }

    public async Task<IEnumerable<ActionItem>> GetActionItemsAsync(ImportConfiguration config)
    {
        List<ActionItem> items = new();

        bool purgingActions = false;

        if (config.Purges?.Any() ?? false)
        {
            foreach (var name in config.Purges)
            {
                purgingActions = purgingActions | name.Equals(PurgeName.ActionItems);

                items.Add(new ActionItem(ActionNames.Purge, name, null, null, 0));
            }
        }

        // get the undone items from the db - if we're not purging actions.
        if (!purgingActions)
        {
            items.AddRange(await logsDb.GetActionItemsByStatusAsync(ImportActionStatus.UsageRequirementMet
                | ImportActionStatus.Error | ImportActionStatus.InProgress | ImportActionStatus.NotStarted));
        }

        //if (config.Fixes?.Any() ?? false)
        //{
        //    foreach (var name in config.Fixes)
        //    {
        //        items.Add(new ActionItem(ActionNames.Fix, name, null, null, 100)); // send to the end of the list
        //    }
        //}

        if (config.DataRetention?.Any() ?? false)
        {
            items.AddRange(GetDataRentionActions(config.DataRetention));
        }

        if (await importsDb.IsDatabaseEmptyAsync() && (config.OnEmptyDatabase?.Any() ?? false))
        {
            items.AddRange(ParseActionItems(nameof(config.OnEmptyDatabase), config.OnEmptyDatabase!));
        }

        if (config.AnyDay?.Any() ?? false)
        {
            items.AddRange(ParseActionItems(nameof(config.AnyDay), config.AnyDay!));
        }

        var dayOfWeek = DateTime.UtcNow.DayOfWeek;

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

        return SortActionItems(items);
    }

    public Task SaveActionItemsAsync(IEnumerable<ActionItem> actions, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return logsDb.SaveActionItemsAsync(actions, cancellationToken);
    }

    private static IEnumerable<ActionItem> ParseActionItems(string parent, ImportActions[]? actions)
    {
        if (actions?.Any() ?? false)
        {
            foreach (var action in actions)
            {
                if (action.Skip.GetValueOrDefault())
                {
                    yield return new ActionItem(ActionNames.Skip, parent);
                    continue;
                }

                if (action.DataTypes?.Contains(DataTypes.Exchanges) ?? false)
                {
                    yield return new ActionItem(ActionNames.Import,
                        DataTypes.Exchanges, DataTypeScopes.Full.ToString(),
                        DataTypes.Exchanges, action.Priority);
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
                    //            yield return new ActionItem(ActionNames.Import, exchange.Name, action.Scope, dataType, action.Priority);
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
        }
    }

    private readonly Regex textToTimeRegex = new(@"(\d+)\s+(year|month|week|day)", RegexOptions.Singleline | RegexOptions.IgnoreCase);

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

        if (actions.Any(a => a.ActionName == ActionNames.Skip))
        {
            actions.RemoveAll(r => r.ActionName == ActionNames.Import);
        }

        if (!actions.Any()) { return Enumerable.Empty<ActionItem>(); }

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

    public static int ActionComparison(ActionItem item1, ActionItem item2)
    {
        if (ReferenceEquals(item1, item2)) return 0;

        int result = item1.Priority.CompareTo(item2.Priority);

        if (result == 0) { result = item1.TargetScopeValue.GetValueOrDefault().CompareTo(item2.TargetScopeValue.GetValueOrDefault()); }
        if (result == 0) { result = item1.TargetDataTypeSortValue.GetValueOrDefault().CompareTo(item2.TargetDataTypeSortValue.GetValueOrDefault()); }

        return result;
    }
}
