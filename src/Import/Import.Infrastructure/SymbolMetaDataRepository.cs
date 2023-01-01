using System.Collections.ObjectModel;

namespace Import.Infrastructure;

internal static class SymbolMetaDataRepository
{
    static readonly SymbolMetaDataCollection metaData = new();

    public static void SetItems(SymbolMetaData[] symbolMetaData)
    {
        metaData.Clear();
        metaData.AddRange(symbolMetaData);
    }

    public static SymbolMetaData[] GetAll() => metaData.ToArray();

    public static string? GetExchangeForSymbol(string symbol) =>
        metaData.FirstOrDefault(d => d.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase))?.Exchange;

    public static IEnumerable<SymbolMetaData> Find(Predicate<SymbolMetaData> predicate) => metaData.Where(d => predicate(d));

    public static void AddOrUpdate(SymbolMetaData symbolMetaData)
    {
        if (metaData.ContainsKey(symbolMetaData.Code))
        {
            metaData[symbolMetaData.Code].LastUpdated = DateTime.UtcNow;
        }
        else
        {
            metaData.Add(symbolMetaData);
        }
    }
}

internal class SymbolMetaDataCollection : KeyedCollection<string, SymbolMetaData>
{
    public void AddRange(IEnumerable<SymbolMetaData> items)
    {
        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    public bool ContainsKey(string key) => Items.Any(i => i.Code.Equals(key, StringComparison.InvariantCultureIgnoreCase));

    protected override string GetKeyForItem(SymbolMetaData item)
    {
        return item.Code;
    }
}
