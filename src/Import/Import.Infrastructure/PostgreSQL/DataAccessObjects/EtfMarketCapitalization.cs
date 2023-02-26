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

        // These TryParse statements are necessary because these values can sometimes arrive as "-".

        if (double.TryParse(marketCapitalization.Mega, out double mega))
        {
            Mega = mega;
        }

        if (double.TryParse(marketCapitalization.Big, out double big))
        {
            Big = big;
        }

        if (double.TryParse(marketCapitalization.Medium, out double medium))
        {
            Medium = medium;
        }

        if (double.TryParse(marketCapitalization.Small, out double small))
        {
            Small = small;
        }

        if (double.TryParse(marketCapitalization.Micro, out double micro))
        {
            Micro = micro;
        }

        UtcTimestamp = DateTime.UtcNow;
    }

    public EtfMarketCapitalization(
        Guid etfId,
        double? mega,
        double? big,
        double? medium,
        double? small,
        double? micro,
        DateTime? utcTimestamp = null)
    {
        EtfId = etfId;
        Mega = mega;
        Big = big;
        Medium = medium;
        Small = small;
        Micro = micro;
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

    [ColumnWithKey("utc_timestamp", Order = 7, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
