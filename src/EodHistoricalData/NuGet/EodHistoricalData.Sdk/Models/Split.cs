using System.Text.Json.Serialization;

namespace EodHistoricalData.Sdk.Models;

/// <summary>
/// Represents an item in the collection of splits provided by the EOD API.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/api-splits-dividends/"/>
/// </summary>
public struct Split
{
    public DateOnly Date;
    [JsonPropertyName("split")]
    public string SplitText;
}
