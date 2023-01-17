using Shibusa.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table("symbols", Schema = "public")]
internal sealed class Symbol
{
    public Symbol(EodHistoricalData.Sdk.Models.Symbol symbol, string exchange)
    {
        Code = $"{symbol.Code}.{exchange}";
        CodeSymbol = symbol.Code;
        Name = symbol.Name ?? throw new ArgumentException($"{nameof(symbol.Name)} cannot be null");
        Country = symbol.Country ?? throw new ArgumentException($"{nameof(symbol.Country)} cannot be null");
        Exchange = symbol.Exchange ?? EodHistoricalData.Sdk.Constants.UnknownValue;
        Currency = symbol.Currency ?? throw new ArgumentException($"{nameof(symbol.Currency)} cannot be null");
        Type = symbol.Type ?? throw new ArgumentException($"{nameof(symbol.Type)} cannot be null");
        HasOptions = null;
        UtcTimestamp = DateTime.UtcNow;
    }

    public Symbol(string code,
        string symbol,
        string exchange,
        string name,
        string country,
        string currency,
        string type,
        bool? hasOptions,
        DateTime utcTimestamp)
    {
        Code = code;
        CodeSymbol = symbol;
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

    [ColumnWithKey("symbol", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string CodeSymbol { get; }

    [ColumnWithKey("exchange", Order = 3, TypeName = "text", IsPartOfKey = true)]
    public string Exchange { get; }

    [ColumnWithKey("name", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string Name { get; }

    [ColumnWithKey("country", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string Country { get; }

    [ColumnWithKey("currency", Order = 6, TypeName = "text", IsPartOfKey = false)]
    public string Currency { get; }

    [ColumnWithKey("type", Order = 7, TypeName = "text", IsPartOfKey = false)]
    public string Type { get; }

    [ColumnWithKey("has_options", Order = 8, TypeName = "boolean", IsPartOfKey = false)]
    public bool? HasOptions { get; }

    [ColumnWithKey("utc_timestamp", Order = 9, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}