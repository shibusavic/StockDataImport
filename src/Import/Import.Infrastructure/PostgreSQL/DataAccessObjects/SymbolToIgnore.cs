using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "symbols_to_ignore", Schema = "public")]
internal class SymbolToIgnore
{
    public SymbolToIgnore(string symbol, string? exchange, string? reason)
    {
        Symbol = symbol;
        Exchange = exchange ?? EodHistoricalData.Sdk.Constants.UnknownValue;
        Reason = reason;
        UtcTimestamp = DateTime.UtcNow;
    }

    public SymbolToIgnore(
        string symbol,
        string exchange,
        string? reason,
        DateTime? utcTimestamp = null)
    {
        Symbol = symbol;
        Exchange = exchange;
        Reason = reason;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("symbol", Order = 1, TypeName = "text", IsPartOfKey = true)]
    public string Symbol { get; }

    [ColumnWithKey("exchange", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string Exchange { get; }

    [ColumnWithKey("reason", Order = 3, TypeName = "text", IsPartOfKey = false)]
    public string? Reason { get; }

    [ColumnWithKey("utc_timestamp", Order = 4, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
