using Shibusa.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "etf_holdings", Schema = "public")]
internal class EtfHolding
{
    public EtfHolding(Guid etfId,
        EodHistoricalData.Sdk.Models.Fundamentals.Etf.Holding holding)
    {
        EtfId = etfId;
        Symbol = holding.Code;
        Exchange = holding.Exchange;
        Name = holding.Name;
        Sector = holding.Sector;
        Industry = holding.Industry;
        Country = holding.Country;
        Region = holding.Region;
        AssetsPercentage = holding.AssetsPercentage;
        CreatedTimestamp = DateTime.UtcNow;
        UtcTimestamp = DateTime.UtcNow;
    }

    public EtfHolding(
        Guid etfId,
        string? symbol,
        string? exchange,
        string? name,
        string? sector,
        string? industry,
        string? country,
        string? region,
        double? assetsPercentage,
        DateTime? createdTimestamp = null,
        DateTime? utcTimestamp = null)
    {
        EtfId = etfId;
        Symbol = symbol;
        Exchange = exchange;
        Name = name;
        Sector = sector;
        Industry = industry;
        Country = country;
        Region = region;
        AssetsPercentage = assetsPercentage;
        CreatedTimestamp = createdTimestamp ?? DateTime.UtcNow;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("etf_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid EtfId { get; }

    [ColumnWithKey("symbol", Order = 2, TypeName = "text", IsPartOfKey = false)]
    public string? Symbol { get; }

    [ColumnWithKey("exchange", Order = 3, TypeName = "text", IsPartOfKey = false)]
    public string? Exchange { get; }

    [ColumnWithKey("name", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string? Name { get; }

    [ColumnWithKey("sector", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string? Sector { get; }

    [ColumnWithKey("industry", Order = 6, TypeName = "text", IsPartOfKey = false)]
    public string? Industry { get; }

    [ColumnWithKey("country", Order = 7, TypeName = "text", IsPartOfKey = false)]
    public string? Country { get; }

    [ColumnWithKey("region", Order = 8, TypeName = "text", IsPartOfKey = false)]
    public string? Region { get; }

    [ColumnWithKey("assets_percentage", Order = 9, TypeName = "double precision", IsPartOfKey = false)]
    public double? AssetsPercentage { get; }

    [ColumnWithKey("created_timestamp", Order = 10, TypeName = "timestamp with time zone", IsPartOfKey = true)]
    public DateTime CreatedTimestamp { get; }

    [ColumnWithKey("utc_timestamp", Order = 11, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
