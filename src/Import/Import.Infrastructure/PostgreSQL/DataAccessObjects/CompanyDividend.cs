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
        ForwardAnnualDividendRate = dividend.ForwardAnnualDividendRate ?? 0D;
        ForwardAnnualDividendYield = dividend.ForwardAnnualDividendYield ?? 0D;
        PayoutRatio = dividend.PayoutRatio ?? 0D;
        DividendDate = dividend.DividendDate?.ToDateTime(TimeOnly.MinValue);
        ExDividendDate = dividend.ExDividendDate?.ToDateTime(TimeOnly.MinValue);
        CreatedTimestamp = DateTime.UtcNow;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyDividend(
        Guid companyId,
        double? forwardAnnualDividendRate,
        double? forwardAnnualDividendYield,
        double? payoutRatio,
        DateTime? dividendDate,
        DateTime? exDividendDate,
        DateTime? createdTimestamp = null,
        DateTime? utcTimestamp = null)
    {
        CompanyId = companyId;
        ForwardAnnualDividendRate = forwardAnnualDividendRate;
        ForwardAnnualDividendYield = forwardAnnualDividendYield;
        PayoutRatio = payoutRatio;
        DividendDate = dividendDate;
        ExDividendDate = exDividendDate;
        CreatedTimestamp = createdTimestamp ?? DateTime.UtcNow;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("forward_annual_dividend_rate", Order = 2, TypeName = "double precision", IsPartOfKey = false)]
    public double? ForwardAnnualDividendRate { get; }

    [ColumnWithKey("forward_annual_dividend_yield", Order = 3, TypeName = "double precision", IsPartOfKey = false)]
    public double? ForwardAnnualDividendYield { get; }

    [ColumnWithKey("payout_ratio", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
    public double? PayoutRatio { get; }

    [ColumnWithKey("dividend_date", Order = 5, TypeName = "date", IsPartOfKey = false)]
    public DateTime? DividendDate { get; }

    [ColumnWithKey("ex_dividend_date", Order = 6, TypeName = "date", IsPartOfKey = false)]
    public DateTime? ExDividendDate { get; }

    [ColumnWithKey("created_timestamp", Order = 7, TypeName = "timestamp with time zone", IsPartOfKey = true)]
    public DateTime CreatedTimestamp { get; }

    [ColumnWithKey("utc_timestamp", Order = 8, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
