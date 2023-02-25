using Import.AppServices.Configuration;
using Import.Infrastructure.Abstractions;
using Import.Infrastructure.Domain;
using Import.Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Shibusa.Extensions;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static Import.Infrastructure.Configuration.Constants;

[assembly: InternalsVisibleTo("Import.AppServices.Tests")]
namespace Import.AppServices;

public class ActionService
{
    private readonly IImportsDbContext importsDb;

    internal ActionService(IImportsDbContext importsDbContext)
    {
        importsDb = importsDbContext;
    }

    public IEnumerable<ActionItem> GetSortedActionItems(ImportConfiguration config, ImportCycle? cycle = null)
    {
        List<ActionItem> items = new();

        if (config.Purges?.Any() ?? false)
        {
            foreach (var name in config.Purges)
            {
                items.Add(new ActionItem(0, ActionNames.Purge, name, null, null, null, cycle));
            }
        }

        if (config.Options.DataRetention?.Any() ?? false)
        {
            items.AddRange(GetDataRentionActions(config.Options.DataRetention, cycle));
        }

        if (config.Exchanges != null)
        {
            foreach (var k in config.Exchanges)
            {
                string exchangeCode = k.Key;

                foreach (var filter in k.Value)
                {
                    if (filter.Key.Equals("Exchanges", StringComparison.InvariantCultureIgnoreCase))
                    {
                        string[] exchanges = filter.Value;

                        if (importsDb.IsDatabaseEmptyAsync().GetAwaiter().GetResult() && (config.OnEmptyDatabase?.Any() ?? false))
                        {
                            items.AddRange(ParseActionItems(nameof(config.OnEmptyDatabase), config.OnEmptyDatabase!,
                                exchangeCode, exchanges, cycle));
                        }

                        if (config.AnyDay?.Any() ?? false)
                        {
                            items.AddRange(ParseActionItems(nameof(config.AnyDay), config.AnyDay!, exchangeCode, exchanges, cycle));
                        }

                        // Using Now instead of UtcNow - want the day of week analysis to be local to the machine.
                        var dayOfWeek = DateTime.Now.DayOfWeek;

                        if ((config.Sunday?.Any() ?? false) && dayOfWeek == DayOfWeek.Sunday)
                        {
                            items.AddRange(ParseActionItems(nameof(config.Sunday), config.Sunday!, exchangeCode, exchanges, cycle));
                        }

                        if ((config.Monday?.Any() ?? false) && dayOfWeek == DayOfWeek.Monday)
                        {
                            items.AddRange(ParseActionItems(nameof(config.Monday), config.Monday!, exchangeCode, exchanges, cycle));
                        }

                        if ((config.Tuesday?.Any() ?? false) && dayOfWeek == DayOfWeek.Tuesday)
                        {
                            items.AddRange(ParseActionItems(nameof(config.Tuesday), config.Tuesday!, exchangeCode, exchanges, cycle));
                        }

                        if ((config.Wednesday?.Any() ?? false) && dayOfWeek == DayOfWeek.Wednesday)
                        {
                            items.AddRange(ParseActionItems(nameof(config.Wednesday), config.Wednesday!, exchangeCode, exchanges, cycle));
                        }

                        if ((config.Thursday?.Any() ?? false) && dayOfWeek == DayOfWeek.Thursday)
                        {
                            items.AddRange(ParseActionItems(nameof(config.Thursday), config.Thursday!, exchangeCode, exchanges, cycle));
                        }

                        if ((config.Friday?.Any() ?? false) && dayOfWeek == DayOfWeek.Friday)
                        {
                            items.AddRange(ParseActionItems(nameof(config.Friday), config.Friday!, exchangeCode, exchanges, cycle));
                        }

                        if ((config.Saturday?.Any() ?? false) && dayOfWeek == DayOfWeek.Saturday)
                        {
                            items.AddRange(ParseActionItems(nameof(config.Saturday), config.Saturday!, exchangeCode, exchanges, cycle));
                        }
                    }
                }
            }
        }

        return SortActionItems(items);
    }

    private static IEnumerable<ActionItem> ParseActionItems(string parent, ImportActions[]? actions,
        string? exchangeCode = null,
        string[]? exchanges = null,
        ImportCycle? cycle = null)
    {
        if (actions?.Any() ?? false)
        {
            foreach (var action in actions)
            {
                var mode = action.Mode ?? Modes.Economy;

                if (action.Skip.GetValueOrDefault())
                {
                    yield return new ActionItem(1, ActionNames.Skip, parent, null, null, null, null, mode);
                    continue;
                }

                if (action.DataTypes?.Contains(DataTypes.Exchanges) ?? false)
                {
                    yield return new ActionItem(2, ActionNames.Import,
                        DataTypes.Exchanges, DataTypeScopes.Full.ToString(),
                        DataTypes.Exchanges, exchangeCode, cycle, mode);
                }

                if (action.IsValidForImport)
                {
                    if ((exchanges?.Any() ?? false) && action.Scope is DataTypeScopes.Full)
                    {
                        foreach (var dataType in action.DataTypes!)
                        {
                            switch (dataType)
                            {
                                case DataTypes.Splits:
                                case DataTypes.Prices:
                                case DataTypes.Dividends:
                                case DataTypes.Fundamentals:
                                case DataTypes.Options:
                                    int pr = dataType switch
                                    {
                                        DataTypes.Splits or DataTypes.Prices or DataTypes.Dividends => 3,
                                        DataTypes.Fundamentals => 4,
                                        _ => 5
                                    };
                                    foreach (var exchange in exchanges)
                                    {
                                        if (!string.IsNullOrWhiteSpace(exchange))
                                        {
                                            yield return new ActionItem(pr, ActionNames.Import, exchange, action.Scope, dataType,
                                                exchangeCode, cycle, mode);
                                        }
                                    }
                                    break;
                                case DataTypes.Earnings:
                                case DataTypes.Trends:
                                case DataTypes.Ipos:
                                case DataTypes.Symbols:
                                    pr = dataType switch
                                    {
                                        DataTypes.Symbols => 2,
                                        _ => 6
                                    };
                                    if (exchangeCode is not null)
                                    {
                                        yield return new ActionItem(pr, ActionNames.Import, exchangeCode,
                                            action.Scope, dataType, exchangeCode, cycle, mode);
                                    }
                                    break;
                                case DataTypes.Exchanges: // do nothing
                                default:
                                    break;
                            };
                        }
                    }

                    if (action.Scope is DataTypeScopes.Bulk or DataTypeScopes.TryBulkThenFull &&
                        exchangeCode is not null)
                    {
                        foreach (var dataType in action.DataTypes!)
                        {
                            if (dataType is DataTypes.Exchanges or DataTypes.Symbols
                                or DataTypes.Fundamentals) { continue; }

                            yield return new ActionItem(2, ActionNames.Import, exchangeCode, action.Scope, dataType,
                                exchangeCode, cycle, mode);
                        }
                    }
                }
            }
        }
    }

    private IEnumerable<ActionItem> GetDataRentionActions(IDictionary<string, string> dataRetention, ImportCycle? cycle = null)
    {
        foreach (var kvp in dataRetention)
        {
            if (Enum.TryParse(kvp.Key, out LogLevel logLevel))
            {
                yield return new ActionItem(10, ActionNames.Truncate, logLevel.GetDescription(),
                    ConvertTextToDateTime(kvp.Value).ToString(), null, null, cycle);
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
            DomainEventPublisher.RaiseMessageEvent(null, "Skip instruction encountered; removing all import actions", nameof(SortActionItems));
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

        // When we have "full" imports scheduled, get rid of the other types of imports with the same target.
        var fullImports = results.Where(r => r.ActionName == ActionNames.Import &&
            r.TargetScope == DataTypeScopes.Full);

        foreach (var fullImport in fullImports)
        {
            int removalCount = results.RemoveAll(r => r.ActionName == ActionNames.Import &&
                r.TargetDataType == fullImport.TargetDataType &&
                r.TargetScope is DataTypeScopes.Bulk or DataTypeScopes.TryBulkThenFull);

            if (removalCount > 0)
            {
                DomainEventPublisher.RaiseMessageEvent(null, $"Removed {removalCount} Bulk or TryBulkThenFull actions ({fullImport.TargetDataType ?? "Unknown target data type"}) because equivalent Full actions were discovered.",
                    nameof(SortActionItems));
            }
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
}
