using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "etf_sector_weights", Schema = "public")]
internal class EtfSectorWeight
{
    public EtfSectorWeight(Guid etfId,
        string sector,
        EodHistoricalData.Sdk.Models.Fundamentals.Etf.EquityWeightItem sectorWeight)
    {
        EtfId = etfId;
        Sector = sector;
        EquityPercentage = Convert.ToDouble(sectorWeight.EquityPercentage);
        RelativeToCategory = Convert.ToDouble(sectorWeight.RelativeToCategory);
        UtcTimestamp = DateTime.UtcNow;
    }

    public EtfSectorWeight(
        Guid etfId,
        string? sector,
        double? equityPercentage,
        double? relativeToCategory,
        DateTime? utcTimestamp = null)
    {
        EtfId = etfId;
        Sector = sector;
        EquityPercentage = equityPercentage;
        RelativeToCategory = relativeToCategory;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("etf_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid EtfId { get; }

    [ColumnWithKey("sector", Order = 2, TypeName = "text", IsPartOfKey = false)]
    public string? Sector { get; }

    [ColumnWithKey("equity_percentage", Order = 3, TypeName = "double precision", IsPartOfKey = false)]
    public double? EquityPercentage { get; }

    [ColumnWithKey("relative_to_category", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
    public double? RelativeToCategory { get; }

    [ColumnWithKey("utc_timestamp", Order = 5, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
