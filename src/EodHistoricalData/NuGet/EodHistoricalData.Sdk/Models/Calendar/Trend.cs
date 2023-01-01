namespace EodHistoricalData.Sdk.Models.Calendar;

/// <summary>
/// Represents an earnings trend.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/calendar-upcoming-earnings-ipos-and-splits/"/>
/// </summary>
public struct Trend
{
    public string Code;
    public DateOnly Date;
    public string Period;
    public double? Growth;
    public decimal? EarningsEstimateAvg;
    public decimal? EarningsEstimateLow;
    public decimal? EarningsEstimateHigh;
    public decimal? EarningsEstimateYearAgoEps;
    public double? EarningsEstimateNumberOfAnalysts;
    public double? EarningsEstimateGrowth;
    public decimal? RevenueEstimateAvg;
    public decimal? RevenueEstimateLow;
    public decimal? RevenueEstimateHigh;
    public decimal? RevenueEstimateYearAgoEps;
    public double? RevenueEstimateNumberOfAnalysts;
    public double? RevenueEstimateGrowth;
    public decimal? EpsTrendCurrent;
    public decimal? EpsTrend7DaysAgo;
    public decimal? EpsTrend30DaysAgo;
    public decimal? EpsTrend60DaysAgo;
    public decimal? EpsTrend90DaysAgo;
    public decimal? EpsRevisionsUpLast7Days;
    public decimal? EpsRevisionsUpLast30Days;
    public decimal? EpsRevisionsDownLast30Days;
}