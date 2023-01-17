//namespace Import.Infrastructure;

//internal static class SymbolsToIgnore
//{
//    static readonly HashSet<IgnoredSymbol> symbolsToIgnore = new();

//    public static void SetItems(IEnumerable<IgnoredSymbol> symbols)
//    {
//        symbolsToIgnore.Clear();
//        foreach (var s in symbols)
//        {
//            symbolsToIgnore.Add(s);
//        }
//    }

//    public static bool IsOnList(string symbol) =>
//        symbolsToIgnore.Any(s => s.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase));

//    public static bool IsOnList(string symbol, string exchange) =>
//        symbolsToIgnore.Any(s => s.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase) &&
//            s.Exchange != null &&
//            s.Exchange.Equals(exchange, StringComparison.InvariantCultureIgnoreCase));

//    public static IgnoredSymbol[] GetAll() => symbolsToIgnore.ToArray();
//}

//internal struct IgnoredSymbol
//{
//    public IgnoredSymbol(string symbol, string? exchange, string? reason)
//    {
//        Symbol = symbol;
//        Exchange = exchange ?? EodHistoricalData.Sdk.Constants.UnknownValue;
//        Reason = reason;
//    }

//    public string Symbol;
//    public string? Exchange;
//    public string? Reason;
//}
