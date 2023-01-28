namespace EodHistoricalData.Sdk.Models.Options;

/// <summary>
/// Represents a collection of option contracts.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/stock-options-data/"/>
/// </summary>
public struct ContractCollection
{
    public DateOnly? ExpirationDate;
    public double? ImpliedVolatility;
    public int? PutVolume;
    public int? CallVolume;
    public double? PutCallVolumeRatio;
    public int? PutOpenInterest;
    public int? CallOpenInterest;
    public double? PutCallOpenInterestRatio;
    public int? OptionsCount;
    public IDictionary<string, Contract[]>? Options;
}
