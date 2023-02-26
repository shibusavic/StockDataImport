using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;
namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "etf_performance", Schema = "public")]
internal class EtfPerformance
{
    public EtfPerformance(Guid etfId,
        EodHistoricalData.Sdk.Models.Fundamentals.Etf.Performance performance)
    {
        EtfId = etfId;
        OneYearVolatility = Convert.ToDouble(performance.OneYearVolatility);
        ThreeYearVolatility = Convert.ToDouble(performance.ThreeYearVolatility);
        ThreeYearExpReturn = Convert.ToDouble(performance.ThreeYearExpectedReturn);
        ThreeYearSharpRatio = Convert.ToDouble(performance.ThreeYearSharpRatio);
        ReturnsYtd = Convert.ToDouble(performance.ReturnsYearToDate);
        Returns1Y = Convert.ToDouble(performance.ReturnsOneYear);
        Returns3Y = Convert.ToDouble(performance.ReturnsThreeYear);
        Returns5Y = Convert.ToDouble(performance.ReturnsFiveYear);
        Returns10Y = Convert.ToDouble(performance.ReturnsTenYear);
        UtcTimestamp = DateTime.UtcNow;
    }

    public EtfPerformance(
        Guid etfId,
        double? oneYearVolatility,
        double? threeYearVolatility,
        double? threeYearExpReturn,
        double? threeYearSharpRatio,
        double? returnsYtd,
        double? returns1Y,
        double? returns3Y,
        double? returns5Y,
        double? returns10Y,
        DateTime? utcTimestamp = null)
    {
        EtfId = etfId;
        OneYearVolatility = oneYearVolatility;
        ThreeYearVolatility = threeYearVolatility;
        ThreeYearExpReturn = threeYearExpReturn;
        ThreeYearSharpRatio = threeYearSharpRatio;
        ReturnsYtd = returnsYtd;
        Returns1Y = returns1Y;
        Returns3Y = returns3Y;
        Returns5Y = returns5Y;
        Returns10Y = returns10Y;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("etf_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid EtfId { get; }

    [ColumnWithKey("one_year_volatility", Order = 2, TypeName = "double precision", IsPartOfKey = false)]
    public double? OneYearVolatility { get; }

    [ColumnWithKey("three_year_volatility", Order = 3, TypeName = "double precision", IsPartOfKey = false)]
    public double? ThreeYearVolatility { get; }

    [ColumnWithKey("three_year_exp_return", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
    public double? ThreeYearExpReturn { get; }

    [ColumnWithKey("three_year_sharp_ratio", Order = 5, TypeName = "double precision", IsPartOfKey = false)]
    public double? ThreeYearSharpRatio { get; }

    [ColumnWithKey("returns_ytd", Order = 6, TypeName = "double precision", IsPartOfKey = false)]
    public double? ReturnsYtd { get; }

    [ColumnWithKey("returns_1_y", Order = 7, TypeName = "double precision", IsPartOfKey = false)]
    public double? Returns1Y { get; }

    [ColumnWithKey("returns_3_y", Order = 8, TypeName = "double precision", IsPartOfKey = false)]
    public double? Returns3Y { get; }

    [ColumnWithKey("returns_5_y", Order = 9, TypeName = "double precision", IsPartOfKey = false)]
    public double? Returns5Y { get; }

    [ColumnWithKey("returns_10_y", Order = 10, TypeName = "double precision", IsPartOfKey = false)]
    public double? Returns10Y { get; }

    [ColumnWithKey("utc_timestamp", Order = 11, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
