using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Metrics;
using System.IO;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_addresses", Schema = "public")]
internal class CompanyAddress
{
    public CompanyAddress(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.Address address)
    {
        if (address.Equals(default)) { throw new ArgumentNullException(nameof(address)); }
        CompanyId = companyId;
        DateCaptured = DateTime.UtcNow;
        Street = address.Street ?? "";
        City = address.City ?? "";
        State = address.State ?? "";
        Country = address.Country ?? "";
        PostalCode = address.PostalCode ?? "";
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyAddress(
        Guid companyId,
        DateTime dateCaptured,
        string street,
        string city,
        string state,
        string country,
        string postalCode,
        DateTime utcTimestamp)
    {
        CompanyId = companyId;
        DateCaptured = dateCaptured;
        Street = street;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = false)]
    public DateTime DateCaptured { get; }

    [ColumnWithKey("street", Order = 3, TypeName = "text", IsPartOfKey = true)]
    public string Street { get; }

    [ColumnWithKey("city", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string City { get; }

    [ColumnWithKey("state", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string State { get; }

    [ColumnWithKey("country", Order = 6, TypeName = "text", IsPartOfKey = false)]
    public string Country { get; }

    [ColumnWithKey("postal_code", Order = 7, TypeName = "text", IsPartOfKey = true)]
    public string PostalCode { get; }

    [ColumnWithKey("utc_timestamp", Order = 8, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
