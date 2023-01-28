namespace EodHistoricalData.Sdk.Models;

/// <summary>
/// Represents an item in the collection of exchanges returned by the Exchange API.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/exchanges-api-list-of-tickers-and-trading-hours/"/>
/// </summary>
public struct Exchange
{
    public string Code;
    public string? Name;
    public string? OperatingMic;
    public string? Country;
    public string? Currency;
    public string? CountryIso2;
    public string? CountryIso3;
}