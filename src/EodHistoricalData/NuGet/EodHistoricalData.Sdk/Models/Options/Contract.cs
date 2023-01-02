namespace EodHistoricalData.Sdk.Models.Options;

/// <summary>
/// Represents an options contract.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/stock-options-data/"/>
/// </summary>
public struct Contract
{
    public string ContractName;
    public string ContractSize;
    public string ContractPeriod;
    public string Currency;
    public string Type;
    public string InTheMoney;
    public DateTime LastTradeDateTime;
    public DateOnly ExpirationDate;
    public decimal Strike;
    public decimal? LastPrice;
    public decimal? Bid;
    public decimal? Ask;
    public decimal? Change;
    public double? ChangePercent;
    public int? Volume;
    public int? OpenInterest;
    public double ImpliedVolatility;
    public double Delta;
    public double Gamma;
    public double Theta;
    public double Vega;
    public double Rho;
    public decimal Theoretical;
    public decimal IntrinsicValue;
    public decimal TimeValue;
    public DateTime UpdatedAt;
    public int DaysBeforeExpiration;
}
