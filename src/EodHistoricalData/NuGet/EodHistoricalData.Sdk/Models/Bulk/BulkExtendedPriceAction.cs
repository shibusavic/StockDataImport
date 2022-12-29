using System.Text.Json.Serialization;

namespace EodHistoricalData.Sdk.Models.Bulk;

/// <summary>
/// Represents a single item in the collection of extended price actions provided by the EOD bulk API.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/bulk-api-eod-splits-dividends/"/>
/// </summary>
public struct BulkExtendedPriceAction
{
    public string Code;
    public string Name;
    [JsonPropertyName("exchange_short_name")]
    public string ExchangeShortName;
    public DateOnly Date;
    public decimal Open;
    public decimal High;
    public decimal Low;
    public decimal Close;
    [JsonPropertyName("adjusted_close")]
    public decimal AdjustedClose;
    public double Volume;
    [JsonPropertyName("ema_50d")]
    public decimal Ema50Day;
    [JsonPropertyName("ema_200d")]
    public decimal Ema200Day;
    [JsonPropertyName("hi_250d")]
    public decimal High250Day;
    [JsonPropertyName("lo_250d")]
    public decimal Low250Day;
    [JsonPropertyName("prev_close")]
    public decimal PreviousClose;
    public decimal Change;
    [JsonPropertyName("change_p")]
    public double ChangePercentage;
    [JsonPropertyName("avgvol_14d")]
    public double AverageVolume14Day;
    [JsonPropertyName("avgvol_50d")]
    public double AverageVolume50Day;
    [JsonPropertyName("avgvol_200d")]
    public double AverageVolume200Day;
}
