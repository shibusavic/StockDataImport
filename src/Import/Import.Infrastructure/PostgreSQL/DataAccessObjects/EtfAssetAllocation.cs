using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "etf_asset_allocations", Schema = "public")]
internal class EtfAssetAllocation
{
    public EtfAssetAllocation(Guid etfId,
        string category,
        EodHistoricalData.Sdk.Models.Fundamentals.Etf.AssetAllocationItem assetAllocation)
    {
        EtfId = etfId;
        Category = category;
        LongPercentage = Convert.ToDouble(assetAllocation.LongPercentage);
        ShortPercentage = Convert.ToDouble(assetAllocation.ShortPercentage);
        NetAssetsPercentage = Convert.ToDouble(assetAllocation.NetAssetsPercentage);
        CreatedTimestamp = DateTime.UtcNow;
        UtcTimestamp = DateTime.UtcNow;
    }

    public EtfAssetAllocation(
        Guid etfId,
        string? category,
        double? longPercentage,
        double? shortPercentage,
        double? netAssetsPercentage,
        DateTime? createdTimestamp = null,
        DateTime? utcTimestamp = null)
    {
        EtfId = etfId;
        Category = category;
        LongPercentage = longPercentage;
        ShortPercentage = shortPercentage;
        NetAssetsPercentage = netAssetsPercentage;
        CreatedTimestamp = createdTimestamp ?? DateTime.UtcNow;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }

    [ColumnWithKey("etf_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid EtfId { get; }

    [ColumnWithKey("category", Order = 2, TypeName = "text", IsPartOfKey = false)]
    public string? Category { get; }

    [ColumnWithKey("long_percentage", Order = 3, TypeName = "double precision", IsPartOfKey = false)]
    public double? LongPercentage { get; }

    [ColumnWithKey("short_percentage", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
    public double? ShortPercentage { get; }

    [ColumnWithKey("net_assets_percentage", Order = 5, TypeName = "double precision", IsPartOfKey = false)]
    public double? NetAssetsPercentage { get; }

    [ColumnWithKey("created_timestamp", Order = 6, TypeName = "timestamp with time zone", IsPartOfKey = true)]
    public DateTime CreatedTimestamp { get; }

    [ColumnWithKey("utc_timestamp", Order = 7, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
