using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "etf_world_regions", Schema = "public")]
internal class EtfWorldRegion
{
    public EtfWorldRegion(Guid etfId,
        string region,
        EodHistoricalData.Sdk.Models.Fundamentals.Etf.EquityWeightItem worldRegion)
    {
        EtfId = etfId;
        Region = region;
        EquityPercentage = Convert.ToDouble(worldRegion.EquityPercentage);
        RelativeToCategory = Convert.ToDouble(worldRegion.RelativeToCategory);
        UtcTimestamp = DateTime.UtcNow;
    }

    public EtfWorldRegion(
        Guid etfId,
        string? region,
        double? equityPercentage,
        double? relativeToCategory,
        DateTime? createdTimestamp = null,
        DateTime? utcTimestamp = null)
    {
        EtfId = etfId;
        Region = region;
        EquityPercentage = equityPercentage;
        RelativeToCategory = relativeToCategory;
        CreatedTimestamp = createdTimestamp ?? DateTime.UtcNow;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("etf_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid EtfId { get; }

    [ColumnWithKey("region", Order = 2, TypeName = "text", IsPartOfKey = false)]
    public string? Region { get; }

    [ColumnWithKey("equity_percentage", Order = 3, TypeName = "double precision", IsPartOfKey = false)]
    public double? EquityPercentage { get; }

    [ColumnWithKey("relative_to_category", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
    public double? RelativeToCategory { get; }

    [ColumnWithKey("created_timestamp", Order = 5, TypeName = "timestamp with time zone", IsPartOfKey = true)]
    public DateTime CreatedTimestamp { get; }

    [ColumnWithKey("utc_timestamp", Order = 6, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
