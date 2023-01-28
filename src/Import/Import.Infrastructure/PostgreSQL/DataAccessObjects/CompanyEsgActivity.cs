using Shibusa.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_esg_activities", Schema = "public")]
internal class CompanyEsgActivity
{
    public CompanyEsgActivity(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.ActivityInvolvement esgActivity)
    {
        CompanyId = companyId;
        Activity = esgActivity.Activity ?? throw new ArgumentException($"{nameof(esgActivity)} has no {nameof(Activity)}");
        Involved = esgActivity.Involvement;
        CreatedTimestamp = DateTime.UtcNow;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyEsgActivity(
        Guid companyId,
        string activity,
        bool? involved,
        DateTime? createdTimestamp = null,
        DateTime? utcTimestamp = null)
    {
        CompanyId = companyId;
        Activity = activity;
        Involved = involved;
        CreatedTimestamp = createdTimestamp ?? DateTime.UtcNow;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }

    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("activity", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string Activity { get; }

    [ColumnWithKey("involved", Order = 3, TypeName = "boolean", IsPartOfKey = false)]
    public bool? Involved { get; }

    [ColumnWithKey("created_timestamp", Order = 4, TypeName = "timestamp with time zone", IsPartOfKey = true)]
    public DateTime CreatedTimestamp { get; }

    [ColumnWithKey("utc_timestamp", Order = 5, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
