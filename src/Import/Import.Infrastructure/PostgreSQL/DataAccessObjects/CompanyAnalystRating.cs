using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_analyst_ratings", Schema = "public")]
internal class CompanyAnalystRating
{
    public CompanyAnalystRating(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.AnalystRatings analystRatings)
    {
        CompanyId = companyId;
        Rating = analystRatings.Rating;
        TargetPrice = analystRatings.TargetPrice;
        StrongBuy = analystRatings.StrongBuy;
        Buy = analystRatings.Buy;
        Hold = analystRatings.Hold;
        Sell = analystRatings.Sell;
        StrongSell = analystRatings.StrongSell;
        CreatedTimestamp = DateTime.UtcNow;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyAnalystRating(
        Guid companyId,
        double? rating,
        decimal? targetPrice,
        int? strongBuy,
        int? buy,
        int? hold,
        int? sell,
        int? strongSell,
        DateTime? createdTimestamp = null,
        DateTime? utcTimestamp = null)
    {
        CompanyId = companyId;
        Rating = rating;
        TargetPrice = targetPrice;
        StrongBuy = strongBuy;
        Buy = buy;
        Hold = hold;
        Sell = sell;
        StrongSell = strongSell;
        CreatedTimestamp = createdTimestamp ?? DateTime.UtcNow;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("rating", Order = 2, TypeName = "double precision", IsPartOfKey = false)]
    public double? Rating { get; }

    [ColumnWithKey("target_price", Order = 3, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TargetPrice { get; }

    [ColumnWithKey("strong_buy", Order = 4, TypeName = "integer", IsPartOfKey = false)]
    public int? StrongBuy { get; }

    [ColumnWithKey("buy", Order = 5, TypeName = "integer", IsPartOfKey = false)]
    public int? Buy { get; }

    [ColumnWithKey("hold", Order = 6, TypeName = "integer", IsPartOfKey = false)]
    public int? Hold { get; }

    [ColumnWithKey("sell", Order = 7, TypeName = "integer", IsPartOfKey = false)]
    public int? Sell { get; }

    [ColumnWithKey("strong_sell", Order = 8, TypeName = "integer", IsPartOfKey = false)]
    public int? StrongSell { get; }

    [ColumnWithKey("created_timestamp", Order = 9, TypeName = "timestamp with time zone", IsPartOfKey = true)]
    public DateTime CreatedTimestamp { get; }

    [ColumnWithKey("utc_timestamp", Order = 10, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
