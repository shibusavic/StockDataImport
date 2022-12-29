using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_dividends", Schema = "public")]
internal class CompanyDividend
{
    public CompanyDividend(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.SplitsDividends dividend)
    {
        CompanyId = companyId;
        DateCaptured = DateTime.UtcNow;
        ForwardAnnualDividendRate = dividend.ForwardAnnualDividendRate ?? 0D;
        ForwardAnnualDividendYield = dividend.ForwardAnnualDividendYield ?? 0D;
        PayoutRatio = dividend.PayoutRatio ?? 0D;
        DividendDate = dividend.DividendDate.GetValueOrDefault().ToDateTime(TimeOnly.MinValue);
        ExDividendDate = dividend.ExDividendDate.GetValueOrDefault().ToDateTime(TimeOnly.MinValue);
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyDividend(
        Guid companyId,
        DateTime dateCaptured,
        double forwardAnnualDividendRate,
        double forwardAnnualDividendYield,
        double payoutRatio,
        DateTime dividendDate,
        DateTime exDividendDate,
        DateTime utcTimestamp)
    {
        CompanyId = companyId;
        DateCaptured = dateCaptured;
        ForwardAnnualDividendRate = forwardAnnualDividendRate;
        ForwardAnnualDividendYield = forwardAnnualDividendYield;
        PayoutRatio = payoutRatio;
        DividendDate = dividendDate;
        ExDividendDate = exDividendDate;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get;  }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = true)]
    public DateTime DateCaptured { get;  }

    [ColumnWithKey("forward_annual_dividend_rate", Order = 3, TypeName = "double precision", IsPartOfKey = false)]
    public double ForwardAnnualDividendRate { get;  }

    [ColumnWithKey("forward_annual_dividend_yield", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
    public double ForwardAnnualDividendYield { get;  }

    [ColumnWithKey("payout_ratio", Order = 5, TypeName = "double precision", IsPartOfKey = false)]
    public double PayoutRatio { get;  }

    [ColumnWithKey("dividend_date", Order = 6, TypeName = "date", IsPartOfKey = false)]
    public DateTime DividendDate { get;  }

    [ColumnWithKey("ex_dividend_date", Order = 7, TypeName = "date", IsPartOfKey = false)]
    public DateTime ExDividendDate { get;  }

    [ColumnWithKey("utc_timestamp", Order = 8, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get;  }
}
