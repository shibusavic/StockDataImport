using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "etf_morning_star", Schema = "public")]
internal class EtfMorningStar
{
    public EtfMorningStar(Guid etfId,
        EodHistoricalData.Sdk.Models.Fundamentals.Etf.MorningStar morningStar)
    {
        EtfId = etfId;
        Ratio = Convert.ToInt32(morningStar.Ratio);
        CategoryBenchmark = morningStar.CategoryBenchmark;
        SustainabilityRatio = Convert.ToInt32(morningStar.SustainabilityRatio);
        CreatedTimestamp = DateTime.UtcNow;
        UtcTimestamp = DateTime.UtcNow;
    }

    public EtfMorningStar(
        Guid etfId,
        int? ratio,
        string? categoryBenchmark,
        int? sustainabilityRatio,
        DateTime? createdTimestamp = null,
        DateTime? utcTimestamp = null)
    {
        EtfId = etfId;
        Ratio = ratio;
        CategoryBenchmark = categoryBenchmark;
        SustainabilityRatio = sustainabilityRatio;
        CreatedTimestamp = createdTimestamp ?? DateTime.UtcNow;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("etf_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid EtfId { get; }

    [ColumnWithKey("ratio", Order = 2, TypeName = "integer", IsPartOfKey = false)]
    public int? Ratio { get; }

    [ColumnWithKey("category_benchmark", Order = 3, TypeName = "text", IsPartOfKey = false)]
    public string? CategoryBenchmark { get; }

    [ColumnWithKey("sustainability_ratio", Order = 4, TypeName = "integer", IsPartOfKey = false)]
    public int? SustainabilityRatio { get; }

    [ColumnWithKey("created_timestamp", Order = 5, TypeName = "timestamp with time zone", IsPartOfKey = true)]
    public DateTime CreatedTimestamp { get; }

    [ColumnWithKey("utc_timestamp", Order = 6, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
