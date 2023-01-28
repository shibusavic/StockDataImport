using System.Text.Json.Serialization;

namespace EodHistoricalData.Sdk.Models.Fundamentals.Etf;

/// <summary>
/// <seealso href="https://eodhistoricaldata.com/financial-apis/stock-etfs-fundamental-data-feeds/"/>
/// </summary>
public struct EtfFundamentalsCollection
{
    public General General;

    public Technicals Technicals;

    [JsonPropertyName("etf_data")]
    public EtfData Data;
}
