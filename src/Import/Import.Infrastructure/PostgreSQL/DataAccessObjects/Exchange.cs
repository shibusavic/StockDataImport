using Shibusa.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects
{
    [Table("exchanges", Schema = "public")]
    internal sealed class Exchange
    {
        public Exchange(EodHistoricalData.Sdk.Models.Exchange exchange)
        {
            Name = exchange.Name;
            Code = exchange.Code;
            OperatingMic = exchange.OperatingMic;
            Country = exchange.Country;
            Currency = exchange.Currency;
            CountryIso2 = exchange.CountryIso2;
            CountryIso3 = exchange.CountryIso3;
            UtcTimestamp = DateTime.UtcNow;
        }

        public Exchange(
            string? name,
            string code,
            string? operatingMic,
            string? country,
            string? currency,
            string? countryIso2,
            string? countryIso3,
            DateTime utcTimestamp)
        {
            Name = name;
            Code = code;
            OperatingMic = operatingMic;
            Country = country;
            Currency = currency;
            CountryIso2 = countryIso2;
            CountryIso3 = countryIso3;
            UtcTimestamp = utcTimestamp;
        }

        [ColumnWithKey("name", Order = 1, TypeName = "text", IsPartOfKey = false)]
        public string? Name { get; }

        [ColumnWithKey("code", Order = 2, TypeName = "text", IsPartOfKey = true)]
        public string Code { get; }

        [ColumnWithKey("operating_mic", Order = 3, TypeName = "text", IsPartOfKey = false)]
        public string? OperatingMic { get; }

        [ColumnWithKey("country", Order = 4, TypeName = "text", IsPartOfKey = false)]
        public string? Country { get; }

        [ColumnWithKey("currency", Order = 5, TypeName = "text", IsPartOfKey = false)]
        public string? Currency { get; }

        [ColumnWithKey("country_iso_2", Order = 6, TypeName = "text", IsPartOfKey = false)]
        public string? CountryIso2 { get; }

        [ColumnWithKey("country_iso_3", Order = 7, TypeName = "text", IsPartOfKey = false)]
        public string? CountryIso3 { get; }

        [ColumnWithKey("utc_timestamp", Order = 8, TypeName = "timestamp with time zone", IsPartOfKey = false)]
        public DateTime UtcTimestamp { get; }
    }
}
