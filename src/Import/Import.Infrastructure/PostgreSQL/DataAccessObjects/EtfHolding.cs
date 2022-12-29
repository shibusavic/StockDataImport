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
        DateCaptured = DateTime.UtcNow;
        Symbol = holding.Code;
        Exchange = holding.Exchange;
        Name = holding.Name;
        Sector = holding.Sector;
        Industry = holding.Industry;
        Country = holding.Country;
        Region = holding.Region;
        AssetsPercentage = holding.AssetsPercentage;
        UtcTimestamp = DateTime.UtcNow;
    }

    public EtfHolding(
        Guid etfId,
        DateTime dateCaptured,
        string symbol,
        string exchange,
        string name,
        string sector,
        string industry,
        string country,
        string region,
        double assetsPercentage,
        DateTime utcTimestamp)
    {
        EtfId = etfId;
        DateCaptured = dateCaptured;
        Symbol = symbol;
        Exchange = exchange;
        Name = name;
        Sector = sector;
        Industry = industry;
        Country = country;
        Region = region;
        AssetsPercentage = assetsPercentage;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("etf_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid EtfId { get; }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = true)]
    public DateTime DateCaptured { get; }

    [ColumnWithKey("symbol", Order = 3, TypeName = "text", IsPartOfKey = false)]
    public string Symbol { get; }

    [ColumnWithKey("exchange", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string Exchange { get; }

    [ColumnWithKey("name", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string Name { get; }

    [ColumnWithKey("sector", Order = 6, TypeName = "text", IsPartOfKey = false)]
    public string Sector { get; }

    [ColumnWithKey("industry", Order = 7, TypeName = "text", IsPartOfKey = false)]
    public string Industry { get; }

    [ColumnWithKey("country", Order = 8, TypeName = "text", IsPartOfKey = false)]
    public string Country { get; }

    [ColumnWithKey("region", Order = 9, TypeName = "text", IsPartOfKey = false)]
    public string Region { get; }

    [ColumnWithKey("assets_percentage", Order = 10, TypeName = "double precision", IsPartOfKey = false)]
    public double AssetsPercentage { get; }

    [ColumnWithKey("utc_timestamp", Order = 11, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
