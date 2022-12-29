using Shibusa.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table("symbols", Schema = "public")]
internal sealed class Symbol
{
    internal Symbol(EodHistoricalData.Sdk.Models.Symbol symbol)
    {
        Code = symbol.Code ?? throw new ArgumentException($"{nameof(symbol.Code)} cannot be null");
        Name = symbol.Name ?? throw new ArgumentException($"{nameof(symbol.Name)} cannot be null");
        Country = symbol.Country ?? throw new ArgumentException($"{nameof(symbol.Country)} cannot be null");
        Exchange = symbol.Exchange ?? "None";
        Currency = symbol.Currency ?? throw new ArgumentException($"{nameof(symbol.Currency)} cannot be null");
        Type = symbol.Type ?? throw new ArgumentException($"{nameof(symbol.Type)} cannot be null");
        HasOptions = null;
        UtcTimestamp = DateTime.UtcNow;
    }

    public Symbol(string code,
        string exchange,
        string name,
        string country,
        string currency,
        string type,
        bool? hasOptions,
        DateTime utcTimestamp)
    {
        Code = code;
        Exchange = exchange;
        Name = name;
        Country = country;
        Currency = currency;
        Type = type;
        HasOptions = hasOptions;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("code", Order = 1, TypeName = "text", IsPartOfKey = true)]
    public string Code { get; }

    [ColumnWithKey("exchange", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string Exchange { get; }

    [ColumnWithKey("name", Order = 3, TypeName = "text", IsPartOfKey = false)]
    public string Name { get; }

    [ColumnWithKey("country", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string Country { get; }

    [ColumnWithKey("currency", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string Currency { get; }

    [ColumnWithKey("type", Order = 6, TypeName = "text", IsPartOfKey = false)]
    public string Type { get; }

    [ColumnWithKey("has_options", Order = 7, TypeName = "boolean", IsPartOfKey = false)]
    public bool? HasOptions { get; }

    [ColumnWithKey("utc_timestamp", Order = 8, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}