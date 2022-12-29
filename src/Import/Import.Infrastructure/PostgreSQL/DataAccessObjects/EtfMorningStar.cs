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
        DateCaptured = DateTime.UtcNow;
        Ratio = Convert.ToInt32(morningStar.Ratio);
        CategoryBenchmark = morningStar.CategoryBenchmark;
        SustainabilityRatio = Convert.ToInt32(morningStar.SustainabilityRatio);
        UtcTimestamp = DateTime.UtcNow;
    }

    public EtfMorningStar(
        Guid etfId,
        DateTime dateCaptured,
        int ratio,
        string categoryBenchmark,
        int sustainabilityRatio,
        DateTime utcTimestamp)
    {
        EtfId = etfId;
        DateCaptured = dateCaptured;
        Ratio = ratio;
        CategoryBenchmark = categoryBenchmark;
        SustainabilityRatio = sustainabilityRatio;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("etf_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid EtfId { get; }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = true)]
    public DateTime DateCaptured { get; }

    [ColumnWithKey("ratio", Order = 3, TypeName = "integer", IsPartOfKey = false)]
    public int Ratio { get; }

    [ColumnWithKey("category_benchmark", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string CategoryBenchmark { get; }

    [ColumnWithKey("sustainability_ratio", Order = 5, TypeName = "integer", IsPartOfKey = false)]
    public int SustainabilityRatio { get; }

    [ColumnWithKey("utc_timestamp", Order = 6, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
