using Import.Infrastructure;
using Import.Infrastructure.Domain;
using Microsoft.VisualBasic;
using System;
using System.Collections.ObjectModel;
using static Import.Infrastructure.Configuration.Constants;

namespace Import.Infrastructure;

internal static class SymbolMetaDataRepository
{
    static readonly SymbolMetaDataCollection metaData = new();

    /// <summary>
    /// Overwrites internal collection of <see cref="SymbolMetaDataCollection"/>.
    /// </summary>
    /// <param name="symbolMetaData"></param>
    public static void SetItems(IEnumerable<SymbolMetaData> symbolMetaData)
    {
        metaData.Clear();
        metaData.AddRange(symbolMetaData);

        Count = metaData.Count;
    }

    public static SymbolMetaData[] GetAll() => metaData.ToArray();

    public static int Count { get; private set; }

    public static void AddOrUpdate(SymbolMetaData symbolMetaData)
    {
        lock (metaData) // Without this lock, this code can crash because the underlying collection can change.
        {
            if (metaData.ContainsKey(symbolMetaData.Code))
            {
                metaData[symbolMetaData.Code].Update(symbolMetaData);
            }
            else
            {
                metaData.Add(symbolMetaData);
                Count++;
            }
        }
    }

    public static int RequiresFundamentalsCount(string? exchange = null)
    {
        return exchange == null ? metaData.Count(s => s.RequiresFundamentalUpdate)
            : metaData.Count(s => s.Exchange == exchange && s.RequiresFundamentalUpdate);
    }

    public static string? GetFirstExchangeForSymbol(string symbol) =>
        metaData.FirstOrDefault(d => d.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase))?.Exchange;

    public static IEnumerable<SymbolMetaData> Find(Predicate<SymbolMetaData> predicate) => metaData.Where(d => predicate(d));

    public static IEnumerable<SymbolMetaData> Find(ActionItem action)
    {
        return action.ActionName == ActionNames.Import
            ? Find(s => s.Exchange == action.TargetName)
            : Enumerable.Empty<SymbolMetaData>();
    }
}

/// <summary>
/// Represents a keyed collection of <see cref="SymbolMetaData"/> instances
/// using <see cref="SymbolMetaData.Code"/> as the key.
/// </summary>
internal class SymbolMetaDataCollection : KeyedCollection<string, SymbolMetaData>
{
    /// <summary>
    /// Add a range of items.
    /// </summary>
    /// <param name="items">A collection of <see cref="SymbolMetaData"/> instances.</param>
    public void AddRange(IEnumerable<SymbolMetaData> items)
    {
        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    /// <summary>
    /// Gets an indicator of whether the collection contains a specific key.
    /// The check is case insensitive.
    /// </summary>
    /// <param name="key">The key; the <see cref="SymbolMetaData.Code"/>.</param>
    /// <returns>A boolean indication of whether the key is contained in the collection.</returns>
    public bool ContainsKey(string key) => Items.Any(i => i.Code.Equals(key, StringComparison.InvariantCultureIgnoreCase));

    protected override string GetKeyForItem(SymbolMetaData item)
    {
        return item.Code;
    }
}
