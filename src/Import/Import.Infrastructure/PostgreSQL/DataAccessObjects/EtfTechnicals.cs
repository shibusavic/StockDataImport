using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "etf_technicals", Schema = "public")]
internal class EtfTechnicals
{
    public EtfTechnicals(Guid etfId,
        EodHistoricalData.Sdk.Models.Fundamentals.Etf.Technicals technicals)
    {
        EtfId = etfId;
        Beta = technicals.Beta.GetValueOrDefault();
        FiftyTwoWeekHigh = technicals.FiftyTwoWeekHigh;
        FiftyTwoWeekLow = technicals.FiftyTwoWeekLow.GetValueOrDefault();
        FiftyDayMa = technicals.FiftyDayMovingAverage.GetValueOrDefault();
        TwoHundredDayMa = technicals.TwoHundredDayMovingAverage.GetValueOrDefault();
        UtcTimestamp = DateTime.UtcNow;
    }

    public EtfTechnicals(
        Guid etfId,
        double? beta,
        decimal? fiftyTwoWeekHigh,
        decimal? fiftyTwoWeekLow,
        decimal? fiftyDayMa,
        decimal? twoHundredDayMa,
        DateTime? utcTimestamp = null)
    {
        EtfId = etfId;
        Beta = beta;
        FiftyTwoWeekHigh = fiftyTwoWeekHigh;
        FiftyTwoWeekLow = fiftyTwoWeekLow;
        FiftyDayMa = fiftyDayMa;
        TwoHundredDayMa = twoHundredDayMa;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }

    [ColumnWithKey("etf_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid EtfId { get; }

    [ColumnWithKey("beta", Order = 2, TypeName = "double precision", IsPartOfKey = false)]
    public double? Beta { get; }

    [ColumnWithKey("fifty_two_week_high", Order = 3, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? FiftyTwoWeekHigh { get; }

    [ColumnWithKey("fifty_two_week_low", Order = 4, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? FiftyTwoWeekLow { get; }

    [ColumnWithKey("fifty_day_ma", Order = 5, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? FiftyDayMa { get; }

    [ColumnWithKey("two_hundred_day_ma", Order = 6, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TwoHundredDayMa { get; }

    [ColumnWithKey("utc_timestamp", Order = 7, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
