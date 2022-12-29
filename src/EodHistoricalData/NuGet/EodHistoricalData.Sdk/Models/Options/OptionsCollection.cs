namespace EodHistoricalData.Sdk.Models.Options;

/// <summary>
/// Represents a collection of option contract collections.
/// </summary>
public struct OptionsCollection
{
    public string Code;
    public string Exchange;
    public DateOnly LastTradeDate;
    public decimal LastTradePrice;
    public ContractCollection[] Data;

    public static OptionsCollection Empty => new()
    {
        Data = Array.Empty<ContractCollection>()
    };
}
