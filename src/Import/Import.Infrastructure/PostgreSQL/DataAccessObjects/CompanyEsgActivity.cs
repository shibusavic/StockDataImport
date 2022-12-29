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
        DateCaptured = DateTime.UtcNow;
        Activity = esgActivity.Activity;
        Involved = esgActivity.Involvement;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyEsgActivity(
        Guid companyId,
        DateTime dateCaptured,
        string activity,
        bool involved,
        DateTime utcTimestamp)
    {
        CompanyId = companyId;
        DateCaptured = dateCaptured;
        Activity = activity;
        Involved = involved;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = true)]
    public DateTime DateCaptured { get; }

    [ColumnWithKey("activity", Order = 3, TypeName = "text", IsPartOfKey = true)]
    public string Activity { get; }

    [ColumnWithKey("involved", Order = 4, TypeName = "boolean", IsPartOfKey = false)]
    public bool Involved { get; }

    [ColumnWithKey("utc_timestamp", Order = 5, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
