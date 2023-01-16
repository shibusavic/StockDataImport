using System.Collections.ObjectModel;

namespace Import.Infrastructure;

internal static class SymbolMetaDataRepository
{
    static readonly SymbolMetaDataCollection metaData = new();

    public static void SetItems(SymbolMetaData[] symbolMetaData)
    {
        metaData.Clear();
        metaData.AddRange(symbolMetaData);
     
        Count = metaData.Count;
        OptionsCount = metaData.Count(d => d.HasOptions);
        RequiresFundamentalsCount = metaData.Count(d => d.RequiresFundamentalUpdate);
    }

    public static SymbolMetaData[] GetAll() => metaData.ToArray();

    public static int Count { get; private set; }

    public static int OptionsCount { get; private set; }

    public static int RequiresFundamentalsCount { get; private set; }

    public static string? GetFirstExchangeForSymbol(string symbol) =>
        metaData.FirstOrDefault(d => d.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase))?.Exchange;

    public static IEnumerable<SymbolMetaData> Find(Predicate<SymbolMetaData> predicate) => metaData.Where(d => predicate(d));

    public static void AddOrUpdate(SymbolMetaData symbolMetaData)
    {
        if (metaData.ContainsKey(symbolMetaData.Code))
        {
            metaData[symbolMetaData.Code].LastUpdatedCompany = symbolMetaData.LastUpdatedCompany;
            metaData[symbolMetaData.Code].LastUpdatedOptions = symbolMetaData.LastUpdatedOptions;
            metaData[symbolMetaData.Code].LastUpdated = DateTime.UtcNow;
            metaData[symbolMetaData.Code].LastUpdatedIncomeStatement = symbolMetaData.LastUpdatedIncomeStatement;
        }
        else
        {
            metaData.Add(symbolMetaData);
            if (symbolMetaData.HasOptions) OptionsCount++;
            if (symbolMetaData.RequiresFundamentalUpdate) RequiresFundamentalsCount++;
            Count++;
        }
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
