using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_holders", Schema = "public")]
internal class CompanyHolder
{
    public CompanyHolder(Guid companyId,
        string type,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.Institution institution)
    {
        CompanyId = companyId;
        DateCaptured = DateTime.UtcNow;
        HolderType = type;
        Name = institution.Name;
        Date = institution.Date.ToDateTime(TimeOnly.MinValue);
        TotalShares = institution.TotalShares;
        TotalAssets = institution.TotalAssets;
        CurrentShares = institution.CurrentShares;
        Change = institution.Change;
        ChangePercentage = institution.ChangePercentage;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyHolder(
        Guid companyId,
        DateTime dateCaptured,
        string holderType,
        string name,
        DateTime date,
        double totalShares,
        double totalAssets,
        int currentShares,
        int change,
        double changePercentage,
        DateTime utcTimestamp)
    {
        CompanyId = companyId;
        DateCaptured = dateCaptured;
        HolderType = holderType;
        Name = name;
        Date = date;
        TotalShares = totalShares;
        TotalAssets = totalAssets;
        CurrentShares = currentShares;
        Change = change;
        ChangePercentage = changePercentage;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = false)]
    public DateTime DateCaptured { get; }

    [ColumnWithKey("holder_type", Order = 3, TypeName = "text", IsPartOfKey = true)]
    public string HolderType { get; }

    [ColumnWithKey("name", Order = 4, TypeName = "text", IsPartOfKey = true)]
    public string Name { get; }

    [ColumnWithKey("date", Order = 5, TypeName = "date", IsPartOfKey = true)]
    public DateTime Date { get; }

    [ColumnWithKey("total_shares", Order = 6, TypeName = "double precision", IsPartOfKey = false)]
    public double TotalShares { get; }

    [ColumnWithKey("total_assets", Order = 7, TypeName = "double precision", IsPartOfKey = false)]
    public double TotalAssets { get; }

    [ColumnWithKey("current_shares", Order = 8, TypeName = "integer", IsPartOfKey = false)]
    public int CurrentShares { get; }

    [ColumnWithKey("change", Order = 9, TypeName = "integer", IsPartOfKey = false)]
    public int Change { get; }

    [ColumnWithKey("change_percentage", Order = 10, TypeName = "double precision", IsPartOfKey = false)]
    public double ChangePercentage { get; }

    [ColumnWithKey("utc_timestamp", Order = 11, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
