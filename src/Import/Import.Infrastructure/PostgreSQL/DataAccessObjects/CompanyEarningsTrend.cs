using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_earnings_trends", Schema = "public")]
internal class CompanyEarningsTrend
{
    public CompanyEarningsTrend(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.EarningsTrend earningsTrend)
    {
        CompanyId = companyId;
        DateCaptured = DateTime.UtcNow;
        Date = earningsTrend.Date.ToDateTime(TimeOnly.MinValue);
        Period = earningsTrend.Period;
        Growth = earningsTrend.Growth.GetValueOrDefault();
        EarningsEstimateAvg = earningsTrend.EarningsEstimateAvg;
        EarningsEstimateLow = earningsTrend.EarningsEstimateLow;
        EarningsEstimateHigh = earningsTrend.EarningsEstimateHigh;
        EarningsEstimateYearAgoEps = earningsTrend.EarningsEstimateYearAgoEps;
        EarningsEstimateNumberOfAnalysts = earningsTrend.EarningsEstimateNumberOfAnalysts;
        EarningsEstimateGrowth = (double)earningsTrend.EarningsEstimateGrowth.GetValueOrDefault();
        RevenueEstimateAvg = earningsTrend.RevenueEstimateAvg;
        RevenueEstimateLow = earningsTrend.RevenueEstimateLow;
        RevenueEstimateHigh = earningsTrend.RevenueEstimateHigh;
        RevenueEstimateYearAgoEps = earningsTrend.RevenueEstimateYearAgoEps;
        RevenueEstimateNumberOfAnalysts = earningsTrend.RevenueEstimateNumberOfAnalysts;
        RevenueEstimateGrowth = (double)earningsTrend.RevenueEstimateGrowth.GetValueOrDefault();
        EpsTrendCurrent = earningsTrend.EpsTrendCurrent;
        EpsTrend7DaysAgo = earningsTrend.EpsTrend7DaysAgo;
        EpsTrend30DaysAgo = earningsTrend.EpsTrend30DaysAgo;
        EpsTrend60DaysAgo = earningsTrend.EpsTrend60DaysAgo;
        EpsTrend90DaysAgo = earningsTrend.EpsTrend90DaysAgo;
        EpsRevisionsUpLast7Days = earningsTrend.EpsRevisionsUpLast7Days;
        EpsRevisionsUpLast30Days = earningsTrend.EpsRevisionsUpLast30Days;
        EpsRevisionsDownLast7Days = earningsTrend.EpsRevisionsDownLast7Days;
        EpsRevisionsDownLast30Days = earningsTrend.EpsRevisionsDownLast30Days;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyEarningsTrend(
        Guid companyId,
        DateTime dateCaptured,
        DateTime date,
        string period,
        double growth,
        decimal? earningsEstimateAvg,
        decimal? earningsEstimateLow,
        decimal? earningsEstimateHigh,
        decimal? earningsEstimateYearAgoEps,
        decimal? earningsEstimateNumberOfAnalysts,
        double? earningsEstimateGrowth,
        decimal? revenueEstimateAvg,
        decimal? revenueEstimateLow,
        decimal? revenueEstimateHigh,
        decimal? revenueEstimateYearAgoEps,
        decimal? revenueEstimateNumberOfAnalysts,
        double? revenueEstimateGrowth,
        decimal? epsTrendCurrent,
        decimal? epsTrend7DaysAgo,
        decimal? epsTrend30DaysAgo,
        decimal? epsTrend60DaysAgo,
        decimal? epsTrend90DaysAgo,
        decimal? epsRevisionsUpLast7Days,
        decimal? epsRevisionsUpLast30Days,
        decimal? epsRevisionsDownLast7Days,
        decimal? epsRevisionsDownLast30Days,
        DateTime utcTimestamp)
    {
        CompanyId = companyId;
        DateCaptured = dateCaptured;
        Date = date;
        Period = period;
        Growth = growth;
        EarningsEstimateAvg = earningsEstimateAvg;
        EarningsEstimateLow = earningsEstimateLow;
        EarningsEstimateHigh = earningsEstimateHigh;
        EarningsEstimateYearAgoEps = earningsEstimateYearAgoEps;
        EarningsEstimateNumberOfAnalysts = earningsEstimateNumberOfAnalysts;
        EarningsEstimateGrowth = earningsEstimateGrowth;
        RevenueEstimateAvg = revenueEstimateAvg;
        RevenueEstimateLow = revenueEstimateLow;
        RevenueEstimateHigh = revenueEstimateHigh;
        RevenueEstimateYearAgoEps = revenueEstimateYearAgoEps;
        RevenueEstimateNumberOfAnalysts = revenueEstimateNumberOfAnalysts;
        RevenueEstimateGrowth = revenueEstimateGrowth;
        EpsTrendCurrent = epsTrendCurrent;
        EpsTrend7DaysAgo = epsTrend7DaysAgo;
        EpsTrend30DaysAgo = epsTrend30DaysAgo;
        EpsTrend60DaysAgo = epsTrend60DaysAgo;
        EpsTrend90DaysAgo = epsTrend90DaysAgo;
        EpsRevisionsUpLast7Days = epsRevisionsUpLast7Days;
        EpsRevisionsUpLast30Days = epsRevisionsUpLast30Days;
        EpsRevisionsDownLast7Days = epsRevisionsDownLast7Days;
        EpsRevisionsDownLast30Days = epsRevisionsDownLast30Days;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get;  }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = false)]
    public DateTime DateCaptured { get;  }

    [ColumnWithKey("date", Order = 3, TypeName = "date", IsPartOfKey = true)]
    public DateTime Date { get;  }

    [ColumnWithKey("period", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string Period { get;  }

    [ColumnWithKey("growth", Order = 5, TypeName = "double precision", IsPartOfKey = false)]
    public double Growth { get;  }

    [ColumnWithKey("earnings_estimate_avg", Order = 6, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EarningsEstimateAvg { get;  }

    [ColumnWithKey("earnings_estimate_low", Order = 7, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EarningsEstimateLow { get;  }

    [ColumnWithKey("earnings_estimate_high", Order = 8, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EarningsEstimateHigh { get;  }

    [ColumnWithKey("earnings_estimate_year_ago_eps", Order = 9, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EarningsEstimateYearAgoEps { get;  }

    [ColumnWithKey("earnings_estimate_number_of_analysts", Order = 10, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EarningsEstimateNumberOfAnalysts { get;  }

    [ColumnWithKey("earnings_estimate_growth", Order = 11, TypeName = "double precision", IsPartOfKey = false)]
    public double? EarningsEstimateGrowth { get;  }

    [ColumnWithKey("revenue_estimate_avg", Order = 12, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? RevenueEstimateAvg { get;  }

    [ColumnWithKey("revenue_estimate_low", Order = 13, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? RevenueEstimateLow { get;  }

    [ColumnWithKey("revenue_estimate_high", Order = 14, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? RevenueEstimateHigh { get;  }

    [ColumnWithKey("revenue_estimate_year_ago_eps", Order = 15, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? RevenueEstimateYearAgoEps { get;  }

    [ColumnWithKey("revenue_estimate_number_of_analysts", Order = 16, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? RevenueEstimateNumberOfAnalysts { get;  }

    [ColumnWithKey("revenue_estimate_growth", Order = 17, TypeName = "double precision", IsPartOfKey = false)]
    public double? RevenueEstimateGrowth { get;  }

    [ColumnWithKey("eps_trend_current", Order = 18, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsTrendCurrent { get;  }

    [ColumnWithKey("eps_trend7days_ago", Order = 19, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsTrend7DaysAgo { get;  }

    [ColumnWithKey("eps_trend30days_ago", Order = 20, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsTrend30DaysAgo { get;  }

    [ColumnWithKey("eps_trend60days_ago", Order = 21, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsTrend60DaysAgo { get;  }

    [ColumnWithKey("eps_trend90days_ago", Order = 22, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsTrend90DaysAgo { get;  }

    [ColumnWithKey("eps_revisions_up_last7days", Order = 23, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsRevisionsUpLast7Days { get;  }

    [ColumnWithKey("eps_revisions_up_last30days", Order = 24, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsRevisionsUpLast30Days { get;  }

    [ColumnWithKey("eps_revisions_down_last7days", Order = 25, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsRevisionsDownLast7Days { get;  }

    [ColumnWithKey("eps_revisions_down_last30days", Order = 26, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsRevisionsDownLast30Days { get;  }

    [ColumnWithKey("utc_timestamp", Order = 27, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get;  }
}
