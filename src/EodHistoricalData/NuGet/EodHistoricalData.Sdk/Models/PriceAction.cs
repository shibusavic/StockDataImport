using System.Text.Json.Serialization;

namespace EodHistoricalData.Sdk.Models;

/// <summary>
/// Represents a single item in the collection provided by the EOD historical API.
/// <seealso href="https://eodhistoricaldata.com/financial-apis/api-for-historical-data-and-volumes/"/>
/// </summary>
public struct PriceAction
{
    public DateOnly Date;
    public decimal Open;
    public decimal High;
    public decimal Low;
    public decimal Close;
    [JsonPropertyName("adjusted_close")]
    public decimal AdjustedClose;
    public double Volume;

    /// <summary>
    /// Create a <see cref="PriceAction"/>, adjusted by the provided factor.
    /// This is for creating prices adjusted for splits.
    /// </summary>
    /// <param name="priceAction">The template <see cref="PriceAction"/>.</param>
    /// <param name="adjustmentFactor">The pricing adjustment factor.</param>
    /// <returns>An adjusted <see cref="PriceAction"/>.</returns>
    /// <remarks>Note that <see cref="AdjustedClose"/> is not modified.
    /// I don't find the <see cref="AdjustedClose"/> property all that useful.</remarks>
    public static PriceAction CreateAdjustedPriceAction(PriceAction priceAction, double adjustmentFactor)
    {
        return new PriceAction()
        {
            Date = priceAction.Date,
            AdjustedClose = priceAction.AdjustedClose,
            Open = Math.Round(priceAction.Open * (decimal)adjustmentFactor, 2),
            High = Math.Round(priceAction.High * (decimal)adjustmentFactor, 2),
            Low = Math.Round(priceAction.Low * (decimal)adjustmentFactor, 2),
            Close = Math.Round(priceAction.Close * (decimal)adjustmentFactor, 2),
            Volume = adjustmentFactor > 1
                ? priceAction.Volume /= adjustmentFactor
                : priceAction.Volume *= Math.Ceiling(1D / adjustmentFactor)
        };
    }
}
