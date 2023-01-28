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
        Date = earnings.Date?.ToDateTime(TimeOnly.MinValue) ??
            throw new ArgumentException($"{nameof(earnings)} has no {nameof(Date)}");
        EpsActual = earnings.EpsActual;
        CreatedTimestamp = DateTime.UtcNow;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyEarningsAnnual(
        Guid companyId,
        DateTime date,
        decimal? epsActual,
        DateTime? createdTimestamp = null,
        DateTime? utcTimestamp = null)
    {
        CompanyId = companyId;
        Date = date;
        EpsActual = epsActual;
        CreatedTimestamp = createdTimestamp ?? DateTime.UtcNow;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("date", Order = 2, TypeName = "date", IsPartOfKey = true)]
    public DateTime Date { get; }

    [ColumnWithKey("eps_actual", Order = 3, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsActual { get; }

    [ColumnWithKey("created_timestamp", Order = 4, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime CreatedTimestamp { get; }

    [ColumnWithKey("utc_timestamp", Order = 5, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
