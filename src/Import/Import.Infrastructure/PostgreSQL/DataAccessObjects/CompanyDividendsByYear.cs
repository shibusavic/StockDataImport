using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_dividends_by_year", Schema = "public")]
internal class CompanyDividendsByYear
{
    public CompanyDividendsByYear(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.CountForYear dividendCount)
    {
        CompanyId = companyId;
        Year = dividendCount.Year ?? throw new ArgumentException($"{nameof(dividendCount)} has no {nameof(Year)}");
        Count = dividendCount.Count;
        CreatedTimestamp = DateTime.UtcNow;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyDividendsByYear(
        Guid companyId,
        int year,
        int? count,
        DateTime? createdTimestamp = null,
        DateTime? utcTimestamp = null)
    {
        CompanyId = companyId;
        Year = year;
        Count = count;
        CreatedTimestamp = createdTimestamp ?? DateTime.UtcNow;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("year", Order = 2, TypeName = "integer", IsPartOfKey = true)]
    public int Year { get; }

    [ColumnWithKey("count", Order = 3, TypeName = "integer", IsPartOfKey = false)]
    public int? Count { get; }

    [ColumnWithKey("created_timestamp", Order = 4, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime CreatedTimestamp { get; }

    [ColumnWithKey("utc_timestamp", Order = 5, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
