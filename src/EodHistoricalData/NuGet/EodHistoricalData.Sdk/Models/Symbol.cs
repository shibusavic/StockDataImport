namespace EodHistoricalData.Sdk.Models;

/// <summary>
/// Represents an item returned in the collection of tickers provided by the Exchange API.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/exchanges-api-list-of-tickers-and-trading-hours/"/>
/// </summary>
public struct Symbol
{
    public string? Code;
    public string? Name;
    public string? Country;
    public string? Exchange;
    public string? Currency;
    public string? Type;

    public static Symbol Empty => new();

    public override string ToString()
    {
        string val = $"{Code} {Exchange} {Type}";
        while (val.Contains("  ")) { val = val.Replace("  ", " ").Trim(); }
        return string.IsNullOrWhiteSpace(val) ? "" : val;
    }
}
