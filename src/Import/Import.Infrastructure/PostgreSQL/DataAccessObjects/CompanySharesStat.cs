using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;
namespace Import.Infrastructure.PostgreSQL.DataAccessObjects
{
    [Table(name: "company_shares_stats", Schema = "public")]
    internal class CompanySharesStat
    {
        public CompanySharesStat(Guid companyId,
            EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.SharesStats shareStats)
        {
            CompanyId = companyId;
            DateCaptured = DateTime.UtcNow;
            SharesOutstanding = shareStats.SharesOutstanding.GetValueOrDefault();
            SharesFloat = shareStats.SharesFloat.GetValueOrDefault();
            PercentInsiders = shareStats.PercentInsiders.GetValueOrDefault();
            PercentInstitutions = shareStats.PercentInstitutions.GetValueOrDefault();
            SharesShort = shareStats.SharesShort.GetValueOrDefault();
            SharesShortPriorMonth = shareStats.SharesShortPriorMonth.GetValueOrDefault();
            ShortRatio = shareStats.ShortRatio.GetValueOrDefault();
            ShortPercentOutstanding = shareStats.ShortPercentOutstanding.GetValueOrDefault();
            ShortPercentFloat = shareStats.ShortPercentFloat.GetValueOrDefault();
            UtcTimestamp = DateTime.UtcNow;
        }

        public CompanySharesStat(
            Guid companyId,
            DateTime dateCaptured,
            double sharesOutstanding,
            double sharesFloat,
            double percentInsiders,
            double percentInstitutions,
            double sharesShort,
            double sharesShortPriorMonth,
            double shortRatio,
            double shortPercentOutstanding,
            double shortPercentFloat,
            DateTime utcTimestamp)
        {
            CompanyId = companyId;
            DateCaptured = dateCaptured;
            SharesOutstanding = sharesOutstanding;
            SharesFloat = sharesFloat;
            PercentInsiders = percentInsiders;
            PercentInstitutions = percentInstitutions;
            SharesShort = sharesShort;
            SharesShortPriorMonth = sharesShortPriorMonth;
            ShortRatio = shortRatio;
            ShortPercentOutstanding = shortPercentOutstanding;
            ShortPercentFloat = shortPercentFloat;
            UtcTimestamp = utcTimestamp;
        }


        [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
        public Guid CompanyId { get;  }

        [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = true)]
        public DateTime DateCaptured { get;  }

        [ColumnWithKey("shares_outstanding", Order = 3, TypeName = "double precision", IsPartOfKey = false)]
        public double SharesOutstanding { get;  }

        [ColumnWithKey("shares_float", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
        public double SharesFloat { get;  }

        [ColumnWithKey("percent_insiders", Order = 5, TypeName = "double precision", IsPartOfKey = false)]
        public double PercentInsiders { get;  }

        [ColumnWithKey("percent_institutions", Order = 6, TypeName = "double precision", IsPartOfKey = false)]
        public double PercentInstitutions { get;  }

        [ColumnWithKey("shares_short", Order = 7, TypeName = "double precision", IsPartOfKey = false)]
        public double SharesShort { get;  }

        [ColumnWithKey("shares_short_prior_month", Order = 8, TypeName = "double precision", IsPartOfKey = false)]
        public double SharesShortPriorMonth { get;  }

        [ColumnWithKey("short_ratio", Order = 9, TypeName = "double precision", IsPartOfKey = false)]
        public double ShortRatio { get;  }

        [ColumnWithKey("short_percent_outstanding", Order = 10, TypeName = "double precision", IsPartOfKey = false)]
        public double ShortPercentOutstanding { get;  }

        [ColumnWithKey("short_percent_float", Order = 11, TypeName = "double precision", IsPartOfKey = false)]
        public double ShortPercentFloat { get;  }

        [ColumnWithKey("utc_timestamp", Order = 12, TypeName = "timestamp with time zone", IsPartOfKey = false)]
        public DateTime UtcTimestamp { get;  }
    }
}
