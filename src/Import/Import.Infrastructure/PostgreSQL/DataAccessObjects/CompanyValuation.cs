using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_valuations", Schema = "public")]
internal class CompanyValuation
{
    public CompanyValuation(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.Valuation valuation)
    {
        CompanyId = companyId;
        TrailingPe = valuation.TrailingPe.GetValueOrDefault();
        ForwardPe = valuation.ForwardPe.GetValueOrDefault();
        PriceSalesTtm = valuation.PriceSalesTtm.GetValueOrDefault();
        PriceBookMrq = valuation.PriceBookMrq.GetValueOrDefault();
        EnterpriseValue = valuation.EnterpriseValue.GetValueOrDefault();
        EnterpriseValueRevenue = valuation.EnterpriseValueRevenue.GetValueOrDefault();
        EnterpriseValueEbitda = valuation.EnterpriseValueEbitda.GetValueOrDefault();
        CreatedTimestamp = DateTime.UtcNow;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyValuation(
        Guid companyId,
        double? trailingPe,
        double? forwardPe,
        double? priceSalesTtm,
        double? priceBookMrq,
        decimal? enterpriseValue,
        double? enterpriseValueRevenue,
        double? enterpriseValueEbitda,
        DateTime? createdTimestamp = null,
        DateTime? utcTimestamp = null)
    {
        CompanyId = companyId;
        TrailingPe = trailingPe;
        ForwardPe = forwardPe;
        PriceSalesTtm = priceSalesTtm;
        PriceBookMrq = priceBookMrq;
        EnterpriseValue = enterpriseValue;
        EnterpriseValueRevenue = enterpriseValueRevenue;
        EnterpriseValueEbitda = enterpriseValueEbitda;
        CreatedTimestamp = createdTimestamp ?? DateTime.UtcNow;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }

    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("trailing_pe", Order = 2, TypeName = "double precision", IsPartOfKey = false)]
    public double? TrailingPe { get; }

    [ColumnWithKey("forward_pe", Order = 3, TypeName = "double precision", IsPartOfKey = false)]
    public double? ForwardPe { get; }

    [ColumnWithKey("price_sales_ttm", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
    public double? PriceSalesTtm { get; }

    [ColumnWithKey("price_book_mrq", Order = 5, TypeName = "double precision", IsPartOfKey = false)]
    public double? PriceBookMrq { get; }

    [ColumnWithKey("enterprise_value", Order = 6, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EnterpriseValue { get; }

    [ColumnWithKey("enterprise_value_revenue", Order = 7, TypeName = "double precision", IsPartOfKey = false)]
    public double? EnterpriseValueRevenue { get; }

    [ColumnWithKey("enterprise_value_ebitda", Order = 8, TypeName = "double precision", IsPartOfKey = false)]
    public double? EnterpriseValueEbitda { get; }

    [ColumnWithKey("created_timestamp", Order = 9, TypeName = "timestamp with time zone", IsPartOfKey = true)]
    public DateTime CreatedTimestamp { get; }

    [ColumnWithKey("utc_timestamp", Order = 10, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
