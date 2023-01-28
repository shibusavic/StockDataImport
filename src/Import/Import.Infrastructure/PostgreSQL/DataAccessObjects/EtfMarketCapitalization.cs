using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "etf_market_capitalization", Schema = "public")]
internal class EtfMarketCapitalization
{
    public EtfMarketCapitalization(Guid etfId,
        EodHistoricalData.Sdk.Models.Fundamentals.Etf.MarketCapitalization marketCapitalization)
    {
        EtfId = etfId;
        Mega = Convert.ToDouble(marketCapitalization.Mega);
        Big = Convert.ToDouble(marketCapitalization.Big);
        Medium = Convert.ToDouble(marketCapitalization.Medium);
        Small = Convert.ToDouble(marketCapitalization.Small);
        Micro = Convert.ToDouble(marketCapitalization.Micro);
        CreatedTimestamp = DateTime.UtcNow;
        UtcTimestamp = DateTime.UtcNow;
    }

    public EtfMarketCapitalization(
        Guid etfId,
        double? mega,
        double? big,
        double? medium,
        double? small,
        double? micro,
        DateTime? createdTimestamp = null,
        DateTime? utcTimestamp = null)
    {
        EtfId = etfId;
        Mega = mega;
        Big = big;
        Medium = medium;
        Small = small;
        Micro = micro;
        CreatedTimestamp = createdTimestamp ?? DateTime.UtcNow;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("etf_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid EtfId { get; }

    [ColumnWithKey("mega", Order = 2, TypeName = "double precision", IsPartOfKey = false)]
    public double? Mega { get; }

    [ColumnWithKey("big", Order = 3, TypeName = "double precision", IsPartOfKey = false)]
    public double? Big { get; }

    [ColumnWithKey("medium", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
    public double? Medium { get; }

    [ColumnWithKey("small", Order = 5, TypeName = "double precision", IsPartOfKey = false)]
    public double? Small { get; }

    [ColumnWithKey("micro", Order = 6, TypeName = "double precision", IsPartOfKey = false)]
    public double? Micro { get; }

    [ColumnWithKey("created_timestamp", Order = 7, TypeName = "timestamp with time zone", IsPartOfKey = true)]
    public DateTime CreatedTimestamp { get; }

    [ColumnWithKey("utc_timestamp", Order = 8, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
