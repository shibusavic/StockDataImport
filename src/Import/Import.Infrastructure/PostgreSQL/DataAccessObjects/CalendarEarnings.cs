using Shibusa.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "calendar_earnings", Schema = "public")]
internal class CalendarEarnings
{
    public CalendarEarnings(EodHistoricalData.Sdk.Models.Calendar.Earnings earnings, string exchange)
    {
        Symbol = earnings.Code?.Split('.')[0] ??
            throw new ArgumentException($"{nameof(earnings)} has no {nameof(Symbol)}");
        Exchange = exchange;
        ReportDate = earnings.ReportDate?.ToDateTime(TimeOnly.MinValue);
        EndingDate = earnings.Date?.ToDateTime(TimeOnly.MinValue) ??
            throw new ArgumentException($"{nameof(earnings)} has no {nameof(EndingDate)}");
        BeforeAfterMarket = earnings.BeforeAfterMarket;
        Currency = earnings.Currency;
        Actual = earnings.Actual;
        Estimate = earnings.Estimate;
        Difference = earnings.Difference;
        Percent = earnings.Percent;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CalendarEarnings(
        string symbol,
        string exchange,
        DateTime? reportDate,
        DateTime endingDate,
        string? beforeAfterMarket,
        string? currency,
        decimal? actual,
        decimal? estimate,
        decimal? difference,
        double? percent,
        DateTime? utcTimestamp = null)
    {
        Symbol = symbol;
        Exchange = exchange;
        ReportDate = reportDate;
        EndingDate = endingDate;
        BeforeAfterMarket = beforeAfterMarket;
        Currency = currency;
        Actual = actual;
        Estimate = estimate;
        Difference = difference;
        Percent = percent;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("symbol", Order = 1, TypeName = "text", IsPartOfKey = true)]
    public string Symbol { get; }

    [ColumnWithKey("exchange", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string Exchange { get; }

    [ColumnWithKey("report_date", Order = 3, TypeName = "date", IsPartOfKey = false)]
    public DateTime? ReportDate { get; }

    [ColumnWithKey("ending_date", Order = 4, TypeName = "date", IsPartOfKey = true)]
    public DateTime EndingDate { get; }

    [ColumnWithKey("before_after_market", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string? BeforeAfterMarket { get; }

    [ColumnWithKey("currency", Order = 6, TypeName = "text", IsPartOfKey = false)]
    public string? Currency { get; }

    [ColumnWithKey("actual", Order = 7, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? Actual { get; }

    [ColumnWithKey("estimate", Order = 8, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? Estimate { get; }

    [ColumnWithKey("difference", Order = 9, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? Difference { get; }

    [ColumnWithKey("percent", Order = 10, TypeName = "double precision", IsPartOfKey = false)]
    public double? Percent { get; }

    [ColumnWithKey("utc_timestamp", Order = 11, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
