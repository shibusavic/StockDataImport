namespace EodHistoricalData.Sdk.Models.Calendar;

/// <summary>
/// Represents a collection of <see cref="Ipo"/>.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/calendar-upcoming-earnings-ipos-and-splits/"/>
/// </summary>
public struct IpoCollection
{
    public string? Type;
    public string? Description;
    public DateOnly? From;
    public DateOnly? To;
    public Ipo[]? Ipos;
}
