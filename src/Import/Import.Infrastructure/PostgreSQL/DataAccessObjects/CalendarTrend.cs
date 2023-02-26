using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "calendar_trends", Schema = "public")]
internal class CalendarTrend
{
    public CalendarTrend(EodHistoricalData.Sdk.Models.Calendar.Trend trend, string exchange)
    {
        Symbol = trend.Code?.Split('.').FirstOrDefault() 
            ?? throw new ArgumentException($"{nameof(trend)} has no {nameof(Symbol)}");
        Date = trend.Date?.ToDateTime(TimeOnly.MinValue) ??
            throw new ArgumentException($"{nameof(trend)} has no {nameof(Date)}");
        Exchange = exchange;
        Period = trend.Period ?? throw new ArgumentException($"{nameof(trend.Period)} is required") ;
        Growth = trend.Growth;
        EarningsEstimateAvg = trend.EarningsEstimateAvg;
        EarningsEstimateLow = trend.EarningsEstimateLow;
        EarningsEstimateHigh = trend.EarningsEstimateHigh;
        EarningsEstimateNumberAnalysts = Convert.ToInt32(trend.EarningsEstimateNumberOfAnalysts);
        EarningsEstimateGrowth = trend.EarningsEstimateGrowth;
        RevenueEstimateAvg = trend.RevenueEstimateAvg;
        RevenueEstimateLow = trend.RevenueEstimateLow;
        RevenueEstimateHigh = trend.RevenueEstimateHigh;
        RevenueEstimateYearAgoEps = trend.RevenueEstimateYearAgoEps;
        RevenueEstimateNumberAnalysts = Convert.ToInt32(trend.RevenueEstimateNumberOfAnalysts);
        RevenueEstimateGrowth = trend.RevenueEstimateGrowth;
        EpsTrendCurrent = trend.EpsTrendCurrent;
        EpsTrend7DaysAgo = trend.EpsTrend7DaysAgo;
        EpsTrend30DaysAgo = trend.EpsTrend30DaysAgo;
        EpsTrend60DaysAgo = trend.EpsTrend60DaysAgo;
        EpsTrend90DaysAgo = trend.EpsTrend90DaysAgo;
        EpsRevisionsUpLast7Days = trend.EpsRevisionsUpLast7Days;
        EpsRevisionsUpLast30Days = trend.EpsRevisionsUpLast30Days;
        EpsRevisionsDownLast30Days = trend.EpsRevisionsDownLast30Days;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CalendarTrend(
        string symbol,
        string exchange,
        DateTime date,
        string period,
        double? growth,
        decimal? earningsEstimateAvg,
        decimal? earningsEstimateLow,
        decimal? earningsEstimateHigh,
        int? earningsEstimateNumberAnalysts,
        double? earningsEstimateGrowth,
        decimal? revenueEstimateAvg,
        decimal? revenueEstimateLow,
        decimal? revenueEstimateHigh,
        decimal? revenueEstimateYearAgoEps,
        int? revenueEstimateNumberAnalysts,
        double? revenueEstimateGrowth,
        decimal? epsTrendCurrent,
        decimal? epsTrend7DaysAgo,
        decimal? epsTrend30DaysAgo,
        decimal? epsTrend60DaysAgo,
        decimal? epsTrend90DaysAgo,
        decimal? epsRevisionsUpLast7Days,
        decimal? epsRevisionsUpLast30Days,
        decimal? epsRevisionsDownLast30Days,
        DateTime? utcTimestamp = null)
    {
        Symbol = symbol;
        Exchange = exchange;
        Date = date;
        Period = period;
        Growth = growth;
        EarningsEstimateAvg = earningsEstimateAvg;
        EarningsEstimateLow = earningsEstimateLow;
        EarningsEstimateHigh = earningsEstimateHigh;
        EarningsEstimateNumberAnalysts = earningsEstimateNumberAnalysts;
        EarningsEstimateGrowth = earningsEstimateGrowth;
        RevenueEstimateAvg = revenueEstimateAvg;
        RevenueEstimateLow = revenueEstimateLow;
        RevenueEstimateHigh = revenueEstimateHigh;
        RevenueEstimateYearAgoEps = revenueEstimateYearAgoEps;
        RevenueEstimateNumberAnalysts = revenueEstimateNumberAnalysts;
        RevenueEstimateGrowth = revenueEstimateGrowth;
        EpsTrendCurrent = epsTrendCurrent;
        EpsTrend7DaysAgo = epsTrend7DaysAgo;
        EpsTrend30DaysAgo = epsTrend30DaysAgo;
        EpsTrend60DaysAgo = epsTrend60DaysAgo;
        EpsTrend90DaysAgo = epsTrend90DaysAgo;
        EpsRevisionsUpLast7Days = epsRevisionsUpLast7Days;
        EpsRevisionsUpLast30Days = epsRevisionsUpLast30Days;
        EpsRevisionsDownLast30Days = epsRevisionsDownLast30Days;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("symbol", Order = 1, TypeName = "text", IsPartOfKey = true)]
    public string Symbol { get; }

    [ColumnWithKey("exchange", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string Exchange { get; }

    [ColumnWithKey("date", Order = 3, TypeName = "date", IsPartOfKey = true)]
    public DateTime Date { get; }

    [ColumnWithKey("period", Order = 4, TypeName = "text", IsPartOfKey = true)]
    public string Period { get; }

    [ColumnWithKey("growth", Order = 5, TypeName = "double precision", IsPartOfKey = false)]
    public double? Growth { get; }

    [ColumnWithKey("earnings_estimate_avg", Order = 6, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EarningsEstimateAvg { get; }

    [ColumnWithKey("earnings_estimate_low", Order = 7, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EarningsEstimateLow { get; }

    [ColumnWithKey("earnings_estimate_high", Order = 8, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EarningsEstimateHigh { get; }

    [ColumnWithKey("earnings_estimate_number_analysts", Order = 9, TypeName = "integer", IsPartOfKey = false)]
    public int? EarningsEstimateNumberAnalysts { get; }

    [ColumnWithKey("earnings_estimate_growth", Order = 10, TypeName = "double precision", IsPartOfKey = false)]
    public double? EarningsEstimateGrowth { get; }

    [ColumnWithKey("revenue_estimate_avg", Order = 11, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? RevenueEstimateAvg { get; }

    [ColumnWithKey("revenue_estimate_low", Order = 12, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? RevenueEstimateLow { get; }

    [ColumnWithKey("revenue_estimate_high", Order = 13, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? RevenueEstimateHigh { get; }

    [ColumnWithKey("revenue_estimate_year_ago_eps", Order = 14, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? RevenueEstimateYearAgoEps { get; }

    [ColumnWithKey("revenue_estimate_number_analysts", Order = 15, TypeName = "integer", IsPartOfKey = false)]
    public int? RevenueEstimateNumberAnalysts { get; }

    [ColumnWithKey("revenue_estimate_growth", Order = 16, TypeName = "double precision", IsPartOfKey = false)]
    public double? RevenueEstimateGrowth { get; }

    [ColumnWithKey("eps_trend_current", Order = 17, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsTrendCurrent { get; }

    [ColumnWithKey("eps_trend_7days_ago", Order = 18, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsTrend7DaysAgo { get; }

    [ColumnWithKey("eps_trend_30days_ago", Order = 19, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsTrend30DaysAgo { get; }

    [ColumnWithKey("eps_trend_60days_ago", Order = 20, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsTrend60DaysAgo { get; }

    [ColumnWithKey("eps_trend_90days_ago", Order = 21, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsTrend90DaysAgo { get; }

    [ColumnWithKey("eps_revisions_up_last7_days", Order = 22, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsRevisionsUpLast7Days { get; }

    [ColumnWithKey("eps_revisions_up_last30_days", Order = 23, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsRevisionsUpLast30Days { get; }

    [ColumnWithKey("eps_revisions_down_last30_days", Order = 24, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsRevisionsDownLast30Days { get; }

    [ColumnWithKey("utc_timestamp", Order = 25, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
