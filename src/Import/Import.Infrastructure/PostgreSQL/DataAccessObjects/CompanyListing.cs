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
        DateCaptured = DateTime.UtcNow;
        Symbol = listing.Code;
        Exchange = listing.Exchange;
        Name = listing.Name;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyListing(
        Guid companyId,
        DateTime dateCaptured,
        string symbol,
        string exchange,
        string name,
        DateTime utcTimestamp)
    {
        CompanyId = companyId;
        DateCaptured = dateCaptured;
        Symbol = symbol;
        Exchange = exchange;
        Name = name;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = true)]
    public DateTime DateCaptured { get; }

    [ColumnWithKey("symbol", Order = 3, TypeName = "text", IsPartOfKey = false)]
    public string Symbol { get; }

    [ColumnWithKey("exchange", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string Exchange { get; }

    [ColumnWithKey("name", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string Name { get; }

    [ColumnWithKey("utc_timestamp", Order = 6, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get;  }
}
