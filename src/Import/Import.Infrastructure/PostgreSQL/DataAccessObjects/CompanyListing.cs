using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_listings", Schema = "public")]
internal class CompanyListing
{
    public CompanyListing(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.Listing listing)
    {
        CompanyId = companyId;
        Symbol = listing.Code;
        Exchange = listing.Exchange;
        Name = listing.Name;
        CreatedTimestamp = DateTime.UtcNow;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyListing(
        Guid companyId,
        string? symbol,
        string? exchange,
        string? name,
        DateTime? createdTimestamp = null,
        DateTime? utcTimestamp = null)
    {
        CompanyId = companyId;
        Symbol = symbol;
        Exchange = exchange;
        Name = name;
        CreatedTimestamp = createdTimestamp ?? DateTime.UtcNow;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }

    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("symbol", Order = 2, TypeName = "text", IsPartOfKey = false)]
    public string? Symbol { get; }

    [ColumnWithKey("exchange", Order = 3, TypeName = "text", IsPartOfKey = false)]
    public string? Exchange { get; }

    [ColumnWithKey("name", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string? Name { get; }

    [ColumnWithKey("created_timestamp", Order = 5, TypeName = "timestamp with time zone", IsPartOfKey = true)]
    public DateTime CreatedTimestamp { get; }

    [ColumnWithKey("utc_timestamp", Order = 6, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
