using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_earnings_history", Schema = "public")]
internal class CompanyEarningsHistory
{
    public CompanyEarningsHistory(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.EarningsHistory earningsHistory)
    {
        CompanyId = companyId;
        DateCaptured = DateTime.UtcNow;
        ReportDate = earningsHistory.ReportDate.ToDateTime(TimeOnly.MinValue);
        Date = earningsHistory.Date.ToDateTime(TimeOnly.MinValue);
        BeforeAfterMarket = earningsHistory.BeforeAfterMarket;
        Currency = earningsHistory.Currency;
        EpsActual = earningsHistory.EpsActual;
        EpsEstimate = earningsHistory.EpsEstimate;
        EpsDifference = earningsHistory.EpsDifference;
        SurprisePercent = earningsHistory.SurprisePercent;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyEarningsHistory(
        Guid companyId,
        DateTime dateCaptured,
        DateTime reportDate,
        DateTime date,
        string beforeAfterMarket,
        string currency,
        decimal? epsActual,
        decimal? epsEstimate,
        decimal? epsDifference,
        double? surprisePercent,
        DateTime utcTimestamp)
    {
        CompanyId = companyId;
        DateCaptured = dateCaptured;
        ReportDate = reportDate;
        Date = date;
        BeforeAfterMarket = beforeAfterMarket;
        Currency = currency;
        EpsActual = epsActual;
        EpsEstimate = epsEstimate;
        EpsDifference = epsDifference;
        SurprisePercent = surprisePercent;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get;  }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = false)]
    public DateTime DateCaptured { get;  }

    [ColumnWithKey("report_date", Order = 3, TypeName = "date", IsPartOfKey = true)]
    public DateTime ReportDate { get;  }

    [ColumnWithKey("date", Order = 4, TypeName = "date", IsPartOfKey = false)]
    public DateTime Date { get;  }

    [ColumnWithKey("before_after_market", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string? BeforeAfterMarket { get;  }

    [ColumnWithKey("currency", Order = 6, TypeName = "text", IsPartOfKey = false)]
    public string Currency { get;  }

    [ColumnWithKey("eps_actual", Order = 7, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsActual { get;  }

    [ColumnWithKey("eps_estimate", Order = 8, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsEstimate { get;  }

    [ColumnWithKey("eps_difference", Order = 9, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsDifference { get;  }

    [ColumnWithKey("surprise_percent", Order = 10, TypeName = "double precision", IsPartOfKey = false)]
    public double? SurprisePercent { get;  }

    [ColumnWithKey("utc_timestamp", Order = 11, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get;  }
}
