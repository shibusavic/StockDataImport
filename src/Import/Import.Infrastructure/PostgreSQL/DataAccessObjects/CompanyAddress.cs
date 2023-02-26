using Shibusa.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_addresses", Schema = "public")]
internal class CompanyAddress
{
    public CompanyAddress(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.Address address)
    {
        if (address.Equals(default)) { throw new ArgumentNullException(nameof(address)); }
        CompanyId = companyId;
        Street = address.Street ?? throw new ArgumentException($"{nameof(address)} has no {nameof(Street)}");
        City = address.City;
        State = address.State;
        Country = address.Country;
        PostalCode = address.PostalCode ?? throw new ArgumentException($"{nameof(address)} has no {nameof(PostalCode)}");
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyAddress(
        Guid companyId,
        string street,
        string? city,
        string? state,
        string? country,
        string postalCode,
        DateTime? utcTimestamp = null)
    {
        CompanyId = companyId;
        Street = street;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("street", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string Street { get; }

    [ColumnWithKey("city", Order = 3, TypeName = "text", IsPartOfKey = false)]
    public string? City { get; }

    [ColumnWithKey("state", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string? State { get; }

    [ColumnWithKey("country", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string? Country { get; }

    [ColumnWithKey("postal_code", Order = 6, TypeName = "text", IsPartOfKey = true)]
    public string PostalCode { get; }

    [ColumnWithKey("utc_timestamp", Order = 7, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
