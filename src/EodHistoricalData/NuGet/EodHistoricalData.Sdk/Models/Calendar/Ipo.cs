using System.Text.Json.Serialization;

namespace EodHistoricalData.Sdk.Models.Calendar;

/// <summary>
/// Represents an Initial Public Offering (IPO)
/// <seealso href="https://eodhistoricaldata.com/financial-apis/calendar-upcoming-earnings-ipos-and-splits/"/>
/// </summary>
public struct Ipo
{
    public string Code;
    public string Name;
    public string Exchange;
    public string? Currency;
    [JsonPropertyName("start_date")]
    public DateOnly? StartDate;
    [JsonPropertyName("filing_date")]
    public DateOnly? FilingDate;
    [JsonPropertyName("amended_date")]
    public DateOnly? AmendedDate;
    [JsonPropertyName("price_from")]
    public decimal? PriceFrom;
    [JsonPropertyName("price_to")]
    public decimal? PriceTo;
    [JsonPropertyName("offer_price")]
    public decimal? OfferPrice;
    public long? Shares;
    [JsonPropertyName("deal_type")]
    public string? DealType;
    public string Symbol => Code.Split('.').FirstOrDefault() ?? Code;
}
