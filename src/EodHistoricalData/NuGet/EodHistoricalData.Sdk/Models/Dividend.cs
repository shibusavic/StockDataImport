namespace EodHistoricalData.Sdk.Models;

/// <summary>
/// Represents an item in the collection of dividends provided by the EOD API.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/api-splits-dividends/"/>
/// </summary>
public struct Dividend
{
    public DateOnly Date;
    public DateOnly? DeclarationDate;
    public DateOnly? RecordDate;
    public DateOnly? PaymentDate;
    public string? Period;
    public decimal Value;
    public decimal UnadjustedValue;
    public string Currency;
}
