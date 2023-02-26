using EodHistoricalData.Sdk;
using Shibusa.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "calendar_ipos", Schema = "public")]
internal class CalendarIpo
{
    public CalendarIpo(EodHistoricalData.Sdk.Models.Calendar.Ipo ipo)
    {
        Symbol = ipo.Code?.Split('.').FirstOrDefault() ?? throw new ArgumentException($"Unable to extract symbol from code: {ipo.Code}");
        Exchange = ipo.Exchange?.ToUpper() ?? Constants.UnknownValue;
        Name = ipo.Name;
        Currency = ipo.Currency;
        StartDate = ipo.StartDate?.ToDateTime(TimeOnly.MinValue);
        FilingDate = ipo.FilingDate?.ToDateTime(TimeOnly.MinValue);
        AmendedDate = ipo.AmendedDate?.ToDateTime(TimeOnly.MaxValue);
        PriceFrom = ipo.PriceFrom ?? 0M;
        PriceTo = ipo.PriceTo ?? 0M;
        OfferPrice = ipo.OfferPrice ?? 0M;
        Shares = Convert.ToInt64(ipo.Shares);
        DealType = ipo.DealType ?? Constants.UnknownValue;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CalendarIpo(
        string symbol,
        string exchange,
        string? name,
        string? currency,
        DateTime? startDate,
        DateTime? filingDate,
        DateTime? amendedDate,
        decimal? priceFrom,
        decimal? priceTo,
        decimal? offerPrice,
        int? shares,
        string? dealType,
        DateTime? utcTimestamp = null)
    {
        Symbol = symbol;
        Exchange = exchange;
        Name = name;
        Currency = currency;
        StartDate = startDate;
        FilingDate = filingDate;
        AmendedDate = amendedDate;
        PriceFrom = priceFrom;
        PriceTo = priceTo;
        OfferPrice = offerPrice;
        Shares = shares;
        DealType = dealType;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("symbol", Order = 1, TypeName = "text", IsPartOfKey = true)]
    public string Symbol { get; }

    [ColumnWithKey("exchange", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string Exchange { get; }

    [ColumnWithKey("name", Order = 3, TypeName = "text", IsPartOfKey = false)]
    public string? Name { get; }

    [ColumnWithKey("currency", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string? Currency { get; }

    [ColumnWithKey("start_date", Order = 5, TypeName = "date", IsPartOfKey = false)]
    public DateTime? StartDate { get; }

    [ColumnWithKey("filing_date", Order = 6, TypeName = "date", IsPartOfKey = false)]
    public DateTime? FilingDate { get; }

    [ColumnWithKey("amended_date", Order = 7, TypeName = "date", IsPartOfKey = false)]
    public DateTime? AmendedDate { get; }

    [ColumnWithKey("price_from", Order = 8, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? PriceFrom { get; }

    [ColumnWithKey("price_to", Order = 9, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? PriceTo { get; }

    [ColumnWithKey("offer_price", Order = 10, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? OfferPrice { get; }

    [ColumnWithKey("shares", Order = 11, TypeName = "bigint", IsPartOfKey = false)]
    public long? Shares { get; }

    [ColumnWithKey("deal_type", Order = 12, TypeName = "text", IsPartOfKey = false)]
    public string? DealType { get; }

    [ColumnWithKey("utc_timestamp", Order = 13, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
