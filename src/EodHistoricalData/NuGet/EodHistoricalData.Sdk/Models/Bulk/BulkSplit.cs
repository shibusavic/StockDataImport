namespace EodHistoricalData.Sdk.Models.Bulk;

/// <summary>
/// Represents a single item in the collection of splits provided by the EOD bulk API.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/bulk-api-eod-splits-dividends/"/>
/// </summary>
public struct BulkSplit
{
    public string? Code;
    public string? Exchange;
    public DateOnly? Date;
    public string? Split;
}
