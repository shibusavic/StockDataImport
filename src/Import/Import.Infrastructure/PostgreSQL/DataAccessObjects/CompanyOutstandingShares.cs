using Shibusa.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_outstanding_shares", Schema = "public")]
internal class CompanyOutstandingShares
{
    public CompanyOutstandingShares(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.OutstandingSharesItem outstandingSharesItem)
    {
        CompanyId = companyId;
        Date = outstandingSharesItem.DateFormatted?.ToDateTime(TimeOnly.MinValue) ??
            throw new ArgumentException($"{nameof(outstandingSharesItem)} has no {nameof(Date)}");
        SharesMln = Convert.ToDecimal(outstandingSharesItem.SharesMln);
        Shares = (int?)outstandingSharesItem.Shares;
        CreatedTimestamp = DateTime.UtcNow;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyOutstandingShares(
        Guid companyId,
        DateTime date,
        decimal? sharesMln,
        int? shares,
        DateTime? createdTimestamp = null,
        DateTime? utcTimestamp = null)
    {
        CompanyId = companyId;
        Date = date;
        SharesMln = sharesMln;
        Shares = shares;
        CreatedTimestamp = createdTimestamp ?? DateTime.UtcNow;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }

    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("date", Order = 2, TypeName = "date", IsPartOfKey = true)]
    public DateTime Date { get; }

    [ColumnWithKey("shares_mln", Order = 3, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? SharesMln { get; }

    [ColumnWithKey("shares", Order = 4, TypeName = "bigint", IsPartOfKey = false)]
    public int? Shares { get; }

    [ColumnWithKey("created_timestamp", Order = 5, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime CreatedTimestamp { get; }

    [ColumnWithKey("utc_timestamp", Order = 6, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
