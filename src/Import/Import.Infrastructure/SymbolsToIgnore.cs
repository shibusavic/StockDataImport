using EodHistoricalData.Sdk;
using EodHistoricalData.Sdk.Models;

namespace Import.Infrastructure;

internal static class SymbolsToIgnore
{
    static readonly HashSet<IgnoredSymbol> symbolsToIgnore = new();

    public static void SetItems(IEnumerable<IgnoredSymbol> symbols)
    {
        symbolsToIgnore.Clear();
        foreach (var s in symbols)
        {
            symbolsToIgnore.Add(s);
        }
    }

    public static void Add(IgnoredSymbol ignoredSymbol)
    {
        symbolsToIgnore.Add(ignoredSymbol);
    }

    public static void Add(Symbol symbol, string? reason = null)
    {
        if (symbol.Code != null)
        {
            Add(new IgnoredSymbol(symbol.Code, symbol.Exchange ?? Constants.UnknownValue, reason));
        }
    }

    public static IEnumerable<Symbol> FilterSymbolCollection(IEnumerable<Symbol> symbols)
    {
        foreach (var symbol in symbols)
        {
            if (!IsOnList(symbol))
            {
                yield return symbol;
            }
        }
    }

    public static bool IsOnList(string symbol) =>
        symbolsToIgnore.Any(s => s.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase));

    public static bool IsOnList(string symbol, string exchange) =>
        symbolsToIgnore.Any(s => s.Symbol.Equals(symbol, StringComparison.InvariantCultureIgnoreCase) &&
            s.Exchange != null &&
            s.Exchange.Equals(exchange, StringComparison.InvariantCultureIgnoreCase));

    public static bool IsOnList(Symbol symbol) => IsOnList(symbol.Code ?? "", symbol.Exchange ?? Constants.UnknownValue);

    public static IgnoredSymbol[] GetAll() => symbolsToIgnore.ToArray();
}

public struct IgnoredSymbol
{
    public IgnoredSymbol(string symbol, string? exchange, string? reason)
    {
        Symbol = symbol;
        Exchange = exchange ?? Constants.UnknownValue;
        Reason = reason;
    }

    public string Symbol;
    public string? Exchange;
    public string? Reason;
}
