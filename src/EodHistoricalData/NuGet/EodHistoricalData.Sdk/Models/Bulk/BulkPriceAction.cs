using System.Text.Json.Serialization;

namespace EodHistoricalData.Sdk.Models.Bulk;

/// <summary>
/// Represents a single item in the collection of price actions provided by the EOD bulk API.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/bulk-api-eod-splits-dividends/"/>
/// </summary>
public struct BulkPriceAction
{
    public string? Code;
    [JsonPropertyName("exchange_short_name")]
    public string? ExchangeShortName;
    public DateOnly? Date;
    public decimal? Open;
    public decimal? High;
    public decimal? Low;
    public decimal? Close;
    [JsonPropertyName("adjusted_close")]
    public decimal? AdjustedClose;
    public double? Volume;
}