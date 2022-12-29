using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_outstanding_shares", Schema = "public")]
internal class CompanyOutstandingShares
{
    public CompanyOutstandingShares(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.OutstandingSharesItem outstandingSharesItem)
    {
        CompanyId = companyId;
        DateCaptured = DateTime.UtcNow;
        Date = outstandingSharesItem.DateFormatted.ToDateTime(TimeOnly.MinValue);
        SharesMln = Convert.ToDecimal(outstandingSharesItem.SharesMln);
        Shares = (int)outstandingSharesItem.Shares;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyOutstandingShares(
        Guid companyId,
        DateTime dateCaptured,
        DateTime date,
        decimal sharesMln,
        int shares,
        DateTime utcTimestamp)
    {
        CompanyId = companyId;
        DateCaptured = dateCaptured;
        Date = date;
        SharesMln = sharesMln;
        Shares = shares;
        UtcTimestamp = utcTimestamp;
    }

    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = false)]
    public DateTime DateCaptured { get; }

    [ColumnWithKey("date", Order = 3, TypeName = "date", IsPartOfKey = true)]
    public DateTime Date { get; }

    [ColumnWithKey("shares_mln", Order = 4, TypeName = "numeric", IsPartOfKey = false)]
    public decimal SharesMln { get; }

    [ColumnWithKey("shares", Order = 5, TypeName = "bigint", IsPartOfKey = false)]
    public int Shares { get; }

    [ColumnWithKey("utc_timestamp", Order = 6, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
