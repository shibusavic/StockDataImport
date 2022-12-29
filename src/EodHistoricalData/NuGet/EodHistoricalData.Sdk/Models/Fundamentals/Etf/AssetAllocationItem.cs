using System.Text.Json.Serialization;

namespace EodHistoricalData.Sdk.Models.Fundamentals.Etf;

/// <summary>
/// <seealso href="https://eodhistoricaldata.com/financial-apis/stock-etfs-fundamental-data-feeds/"/>
/// </summary>
public struct AssetAllocationItem
{
    [JsonPropertyName("long_%")]
    public string LongPercentage;
    [JsonPropertyName("short_%")]
    public string ShortPercentage;
    [JsonPropertyName("net_assets_%")]
    public string NetAssetsPercentage;
}
