using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;
namespace Import.Infrastructure.PostgreSQL.DataAccessObjects
{
    [Table(name: "company_valuations", Schema = "public")]
    internal class CompanyValuation
    {
        public CompanyValuation(Guid companyId,
            EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.Valuation valuation)
        {
            CompanyId = companyId;
            DateCaptured = DateTime.UtcNow;
            TrailingPe = valuation.TrailingPe.GetValueOrDefault();
            ForwardPe = valuation.ForwardPe.GetValueOrDefault();
            PriceSalesTtm = valuation.PriceSalesTtm.GetValueOrDefault();
            PriceBookMrq = valuation.PriceBookMrq.GetValueOrDefault();
            EnterpriseValue = valuation.EnterpriseValue.GetValueOrDefault();
            EnterpriseValueRevenue = valuation.EnterpriseValueRevenue.GetValueOrDefault();
            EnterpriseValueEbitda = valuation.EnterpriseValueEbitda.GetValueOrDefault();
            UtcTimestamp = DateTime.UtcNow;
        }

        public CompanyValuation(
            Guid companyId,
            DateTime dateCaptured,
            double trailingPe,
            double forwardPe,
            double priceSalesTtm,
            double priceBookMrq,
            decimal enterpriseValue,
            double enterpriseValueRevenue,
            double enterpriseValueEbitda,
            DateTime utcTimestamp)
        {
            CompanyId = companyId;
            DateCaptured = dateCaptured;
            TrailingPe = trailingPe;
            ForwardPe = forwardPe;
            PriceSalesTtm = priceSalesTtm;
            PriceBookMrq = priceBookMrq;
            EnterpriseValue = enterpriseValue;
            EnterpriseValueRevenue = enterpriseValueRevenue;
            EnterpriseValueEbitda = enterpriseValueEbitda;
            UtcTimestamp = utcTimestamp;
        }


        [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
        public Guid CompanyId { get;  }

        [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = true)]
        public DateTime DateCaptured { get;  }

        [ColumnWithKey("trailing_pe", Order = 3, TypeName = "double precision", IsPartOfKey = false)]
        public double TrailingPe { get;  }

        [ColumnWithKey("forward_pe", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
        public double ForwardPe { get;  }

        [ColumnWithKey("price_sales_ttm", Order = 5, TypeName = "double precision", IsPartOfKey = false)]
        public double PriceSalesTtm { get;  }

        [ColumnWithKey("price_book_mrq", Order = 6, TypeName = "double precision", IsPartOfKey = false)]
        public double PriceBookMrq { get;  }

        [ColumnWithKey("enterprise_value", Order = 7, TypeName = "numeric", IsPartOfKey = false)]
        public decimal EnterpriseValue { get;  }

        [ColumnWithKey("enterprise_value_revenue", Order = 8, TypeName = "double precision", IsPartOfKey = false)]
        public double EnterpriseValueRevenue { get;  }

        [ColumnWithKey("enterprise_value_ebitda", Order = 9, TypeName = "double precision", IsPartOfKey = false)]
        public double EnterpriseValueEbitda { get;  }

        [ColumnWithKey("utc_timestamp", Order = 10, TypeName = "timestamp with time zone", IsPartOfKey = false)]
        public DateTime UtcTimestamp { get;  }
    }
}
