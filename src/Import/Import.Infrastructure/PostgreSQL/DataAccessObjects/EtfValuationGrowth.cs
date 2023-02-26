using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "etf_valuation_growths", Schema = "public")]
internal class EtfValuationGrowth
{
    public EtfValuationGrowth(Guid etfId,
        string category,
        EodHistoricalData.Sdk.Models.Fundamentals.Etf.Valuation valuation)
    {
        EtfId = etfId;
        Category = category;
        PriceProspectiveEarnings = Convert.ToDouble(valuation.PriceProspectiveEarnings);
        PriceBook = Convert.ToDouble(valuation.PriceBook);
        PriceSales = Convert.ToDouble(valuation.PriceSales);
        PriceCashFlow = Convert.ToDouble(valuation.PriceCashFlow);
        DividendYieldFactor = Convert.ToDouble(valuation.DividendYieldFactor);
        UtcTimestamp = DateTime.UtcNow;
    }

    public EtfValuationGrowth(
        Guid etfId,
        string? category,
        double? priceProspectiveEarnings,
        double? priceBook,
        double? priceSales,
        double? priceCashFlow,
        double? dividendYieldFactor,
        DateTime? utcTimestamp = null)
    {
        EtfId = etfId;
        Category = category;
        PriceProspectiveEarnings = priceProspectiveEarnings;
        PriceBook = priceBook;
        PriceSales = priceSales;
        PriceCashFlow = priceCashFlow;
        DividendYieldFactor = dividendYieldFactor;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("etf_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid EtfId { get; }

    [ColumnWithKey("category", Order = 2, TypeName = "text", IsPartOfKey = false)]
    public string? Category { get; }

    [ColumnWithKey("price_prospective_earnings", Order = 3, TypeName = "double precision", IsPartOfKey = false)]
    public double? PriceProspectiveEarnings { get; }

    [ColumnWithKey("price_book", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
    public double? PriceBook { get; }

    [ColumnWithKey("price_sales", Order = 5, TypeName = "double precision", IsPartOfKey = false)]
    public double? PriceSales { get; }

    [ColumnWithKey("price_cash_flow", Order = 6, TypeName = "double precision", IsPartOfKey = false)]
    public double? PriceCashFlow { get; }

    [ColumnWithKey("dividend_yield_factor", Order = 7, TypeName = "double precision", IsPartOfKey = false)]
    public double? DividendYieldFactor { get; }

    [ColumnWithKey("utc_timestamp", Order = 8, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
