namespace EodHistoricalData.Sdk.Models.Bulk;

/// <summary>
/// Represents a single item in the collection of dividends provided by the EOD bulk API.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/bulk-api-eod-splits-dividends/"/>
/// </summary>
public struct BulkDividend
{
    public string Code;
    public string Exchange;
    public DateOnly Date;
    public string Dividend;
    public string Currency;
    public DateOnly? DeclarationDate;
    public DateOnly? RecordDate;
    public DateOnly? PaymentDate;
    public string? Period;
    public string UnadjustedValue;
}
