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
        DateCaptured = DateTime.UtcNow;
        Sector = sector;
        EquityPercentage = Convert.ToDouble(sectorWeight.EquityPercentage);
        RelativeToCategory = Convert.ToDouble(sectorWeight.RelativeToCategory);
        UtcTimestamp = DateTime.UtcNow;
    }

    public EtfSectorWeight(
        Guid etfId,
        DateTime dateCaptured,
        string sector,
        double equityPercentage,
        double relativeToCategory,
        DateTime utcTimestamp)
    {
        EtfId = etfId;
        DateCaptured = dateCaptured;
        Sector = sector;
        EquityPercentage = equityPercentage;
        RelativeToCategory = relativeToCategory;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("etf_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid EtfId { get;  }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = true)]
    public DateTime DateCaptured { get;  }

    [ColumnWithKey("sector", Order = 3, TypeName = "text", IsPartOfKey = false)]
    public string Sector { get;  }

    [ColumnWithKey("equity_percentage", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
    public double EquityPercentage { get;  }

    [ColumnWithKey("relative_to_category", Order = 5, TypeName = "double precision", IsPartOfKey = false)]
    public double RelativeToCategory { get;  }

    [ColumnWithKey("utc_timestamp", Order = 6, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get;  }
}
