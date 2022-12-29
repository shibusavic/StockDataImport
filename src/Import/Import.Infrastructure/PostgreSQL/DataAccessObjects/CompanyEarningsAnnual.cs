using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_earnings_annual", Schema = "public")]
internal class CompanyEarningsAnnual
{
    public CompanyEarningsAnnual(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.EarningsPerShare earnings)
    {
        CompanyId = companyId;
        DateCaptured = DateTime.UtcNow;
        Date = earnings.Date.ToDateTime(TimeOnly.MinValue);
        EpsActual = earnings.EpsActual;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyEarningsAnnual(
        Guid companyId,
        DateTime dateCaptured,
        DateTime date,
        decimal epsActual,
        DateTime utcTimestamp)
    {
        CompanyId = companyId;
        DateCaptured = dateCaptured;
        Date = date;
        EpsActual = epsActual;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get;  }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = false)]
    public DateTime DateCaptured { get;  }

    [ColumnWithKey("date", Order = 3, TypeName = "date", IsPartOfKey = true)]
    public DateTime Date { get;  }

    [ColumnWithKey("eps_actual", Order = 4, TypeName = "numeric", IsPartOfKey = false)]
    public decimal EpsActual { get;  }

    [ColumnWithKey("utc_timestamp", Order = 5, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get;  }
}
