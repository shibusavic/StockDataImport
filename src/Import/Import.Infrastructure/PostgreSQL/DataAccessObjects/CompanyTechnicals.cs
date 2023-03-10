using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_technicals", Schema = "public")]
internal class CompanyTechnicals
{
    public CompanyTechnicals(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.Technicals technicals)
    {
        CompanyId = companyId;
        Beta = technicals.Beta.GetValueOrDefault();
        FiftyTwoWeekHigh = technicals.FiftyTwoWeekHigh.GetValueOrDefault();
        FiftyTwoWeekLow = technicals.FiftyTwoWeekLow.GetValueOrDefault();
        FiftyDayMa = technicals.FiftyDayMovingAverage.GetValueOrDefault();
        TwoHundredDayMa = technicals.TwoHundredDayMovingAverage.GetValueOrDefault();
        SharesShort = technicals.SharesShort.GetValueOrDefault();
        SharesShortPriorMonth = technicals.SharesShortPriorMonth.GetValueOrDefault();
        ShortRatio = technicals.ShortRatio.GetValueOrDefault();
        ShortPercent = technicals.ShortPercent.GetValueOrDefault();
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyTechnicals(
        Guid companyId,
        double? beta,
        decimal? fiftyTwoWeekHigh,
        decimal? fiftyTwoWeekLow,
        decimal? fiftyDayMa,
        decimal? twoHundredDayMa,
        decimal? sharesShort,
        decimal? sharesShortPriorMonth,
        double? shortRatio,
        double? shortPercent,
        DateTime? utcTimestamp = null)
    {
        CompanyId = companyId;
        Beta = beta;
        FiftyTwoWeekHigh = fiftyTwoWeekHigh;
        FiftyTwoWeekLow = fiftyTwoWeekLow;
        FiftyDayMa = fiftyDayMa;
        TwoHundredDayMa = twoHundredDayMa;
        SharesShort = sharesShort;
        SharesShortPriorMonth = sharesShortPriorMonth;
        ShortRatio = shortRatio;
        ShortPercent = shortPercent;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

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

    [ColumnWithKey("shares_short", Order = 7, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? SharesShort { get; }

    [ColumnWithKey("shares_short_prior_month", Order = 8, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? SharesShortPriorMonth { get; }

    [ColumnWithKey("short_ratio", Order = 9, TypeName = "double precision", IsPartOfKey = false)]
    public double? ShortRatio { get; }

    [ColumnWithKey("short_percent", Order = 10, TypeName = "double precision", IsPartOfKey = false)]
    public double? ShortPercent { get; }

    [ColumnWithKey("utc_timestamp", Order = 11, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
