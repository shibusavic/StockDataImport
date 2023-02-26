using Shibusa.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_officers", Schema = "public")]
internal class CompanyOfficer
{
    public CompanyOfficer(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.Officer officer)
    {
        CompanyId = companyId;
        Name = officer.Name ?? EodHistoricalData.Sdk.Constants.UnknownValue;
        Title = officer.Title;
        YearBorn = officer.YearBorn;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyOfficer(
        Guid companyId,
        string? name,
        string? title,
        string? yearBorn,
        DateTime? utcTimestamp = null)
    {
        CompanyId = companyId;
        Name = name;
        Title = title;
        YearBorn = yearBorn;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }

    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("name", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string? Name { get; }

    [ColumnWithKey("title", Order = 3, TypeName = "text", IsPartOfKey = false)]
    public string? Title { get; }

    [ColumnWithKey("year_born", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string? YearBorn { get; }

    [ColumnWithKey("utc_timestamp", Order = 5, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
