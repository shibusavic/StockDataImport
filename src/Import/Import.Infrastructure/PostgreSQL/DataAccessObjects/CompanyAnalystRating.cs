using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_analyst_ratings", Schema = "public")]
internal class CompanyAnalystRating
{
    public CompanyAnalystRating(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.AnalystRatings analystRatings)
        : this(companyId, DateTime.UtcNow, analystRatings.Rating.GetValueOrDefault(),
              analystRatings.TargetPrice.GetValueOrDefault(),
              analystRatings.StrongBuy.GetValueOrDefault(),
              analystRatings.Buy.GetValueOrDefault(),
              analystRatings.Hold.GetValueOrDefault(),
              analystRatings.Sell.GetValueOrDefault(),
              analystRatings.StrongSell.GetValueOrDefault(),
              DateTime.UtcNow)
        { }

    public CompanyAnalystRating(
        Guid companyId,
        DateTime dateCaptured,
        double rating,
        decimal targetPrice,
        int strongBuy,
        int buy,
        int hold,
        int sell,
        int strongSell,
        DateTime utcTimestamp)
    {
        CompanyId = companyId;
        DateCaptured = dateCaptured;
        Rating = rating;
        TargetPrice = targetPrice;
        StrongBuy = strongBuy;
        Buy = buy;
        Hold = hold;
        Sell = sell;
        StrongSell = strongSell;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get;  }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = true)]
    public DateTime DateCaptured { get;  }

    [ColumnWithKey("rating", Order = 3, TypeName = "double precision", IsPartOfKey = false)]
    public double Rating { get;  }

    [ColumnWithKey("target_price", Order = 4, TypeName = "numeric", IsPartOfKey = false)]
    public decimal TargetPrice { get;  }

    [ColumnWithKey("strong_buy", Order = 5, TypeName = "integer", IsPartOfKey = false)]
    public int StrongBuy { get;  }

    [ColumnWithKey("buy", Order = 6, TypeName = "integer", IsPartOfKey = false)]
    public int Buy { get;  }

    [ColumnWithKey("hold", Order = 7, TypeName = "integer", IsPartOfKey = false)]
    public int Hold { get;  }

    [ColumnWithKey("sell", Order = 8, TypeName = "integer", IsPartOfKey = false)]
    public int Sell { get;  }

    [ColumnWithKey("strong_sell", Order = 9, TypeName = "integer", IsPartOfKey = false)]
    public int StrongSell { get;  }

    [ColumnWithKey("utc_timestamp", Order = 10, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get;  }
}
