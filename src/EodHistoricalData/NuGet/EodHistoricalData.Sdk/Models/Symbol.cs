namespace EodHistoricalData.Sdk.Models;

/// <summary>
/// Represents an item returned in the collection of tickers provided by the Exchange API.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/exchanges-api-list-of-tickers-and-trading-hours/"/>
/// </summary>
public struct Symbol : IEquatable<Symbol>
{
    public string Code;
    public string Name;
    public string Country;
    public string Exchange;
    public string Currency;
    public string Type;

    public static Symbol Empty => new();

    public override string ToString()
    {
        return $"{Code} {Exchange} {Type}";
    }

    public override bool Equals(object? obj)
    {
        return obj is Symbol symbol && Equals(symbol);
    }

    public bool Equals(Symbol other)
    {
        return Code == other.Code &&
               Exchange == other.Exchange &&
               Type == other.Type;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Code, Exchange, Type);
    }
}
