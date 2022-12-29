namespace EodHistoricalData.Sdk.Models.Calendar;

/// <summary>
/// Represents a collection of <see cref="Trend"/>.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/calendar-upcoming-earnings-ipos-and-splits/"/>
/// </summary>
public struct TrendCollection
{
    public string Type;
    public string Description;
    public string Symbols;
    public Trend[] Trends;

    public static TrendCollection Empty => new()
    {
        Type = string.Empty,
        Description = string.Empty,
        Symbols = string.Empty,
        Trends = Array.Empty<Trend>()
    };
}
