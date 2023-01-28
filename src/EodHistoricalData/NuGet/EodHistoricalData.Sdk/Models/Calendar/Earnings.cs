using System.Text.Json.Serialization;

namespace EodHistoricalData.Sdk.Models.Calendar;

/// <summary>
/// Represents an earnings report.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/calendar-upcoming-earnings-ipos-and-splits/"/>
/// </summary>
public struct Earnings
{
    public string? Code;

    [JsonPropertyName("report_date")]
    public DateOnly? ReportDate;

    public DateOnly? Date;

    [JsonPropertyName("before_after_market")]
    public string? BeforeAfterMarket;

    public string? Currency;

    public decimal? Actual;

    public decimal? Estimate;

    public decimal? Difference;

    public double? Percent;
}