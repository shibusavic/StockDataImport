using Shibusa.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table("symbols", Schema = "public")]
internal sealed class Symbol
{
    public Symbol(EodHistoricalData.Sdk.Models.Symbol symbol, string exchangeCode)
    {
        Code = $"{symbol.Code}.{exchangeCode}";
        CodeSymbol = symbol.Code;
        Name = symbol.Name ?? throw new ArgumentException($"{nameof(symbol.Name)} cannot be null");
        Country = symbol.Country ?? throw new ArgumentException($"{nameof(symbol.Country)} cannot be null");
        Exchange = symbol.Exchange ?? EodHistoricalData.Sdk.Constants.UnknownValue;
        Currency = symbol.Currency ?? throw new ArgumentException($"{nameof(symbol.Currency)} cannot be null");
        Type = symbol.Type ?? throw new ArgumentException($"{nameof(symbol.Type)} cannot be null");
        UtcTimestamp = DateTime.UtcNow;
    }

    public Symbol(
        string code,
        string? codeSymbol,
        string? exchange,
        string? name,
        string? country,
        string? currency,
        string? type,
        DateTime? utcTimestamp = null)
    {
        Code = code;
        CodeSymbol = codeSymbol;
        Exchange = exchange;
        Name = name;
        Country = country;
        Currency = currency;
        Type = type;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }

    [ColumnWithKey("code", Order = 1, TypeName = "text", IsPartOfKey = true)]
    public string Code { get; }

    [ColumnWithKey("symbol", Order = 2, TypeName = "text", IsPartOfKey = false)]
    public string? CodeSymbol { get; }

    [ColumnWithKey("exchange", Order = 3, TypeName = "text", IsPartOfKey = false)]
    public string? Exchange { get; }

    [ColumnWithKey("name", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string? Name { get; }

    [ColumnWithKey("country", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string? Country { get; }

    [ColumnWithKey("currency", Order = 6, TypeName = "text", IsPartOfKey = false)]
    public string? Currency { get; }

    [ColumnWithKey("type", Order = 7, TypeName = "text", IsPartOfKey = false)]
    public string? Type { get; }

    [ColumnWithKey("utc_timestamp", Order = 8, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}