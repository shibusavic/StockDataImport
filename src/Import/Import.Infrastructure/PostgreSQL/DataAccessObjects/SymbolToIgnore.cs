using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "symbols_to_ignore", Schema = "public")]
internal class SymbolToIgnore
{
    public SymbolToIgnore(string symbol, string? exchange, string? reason)
    {
        Symbol = symbol;
        Exchange = exchange ?? "None";
        Reason = reason;
        DateIgnored = DateTime.UtcNow;
        UtcTimestamp = DateTime.UtcNow;
    }

    public SymbolToIgnore(
        string symbol,
        string exchange,
        DateTime dateIgnored,
        string? reason,
        DateTime utcTimestamp)
    {
        Symbol = symbol;
        Exchange = exchange;
        DateIgnored = dateIgnored;
        Reason = reason;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("symbol", Order = 1, TypeName = "text", IsPartOfKey = true)]
    public string Symbol { get;  }

    [ColumnWithKey("exchange", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string Exchange { get;  }

    [ColumnWithKey("date_ignored", Order = 3, TypeName = "date", IsPartOfKey = false)]
    public DateTime DateIgnored { get;  }

    [ColumnWithKey("reason", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string? Reason { get;  }

    [ColumnWithKey("utc_timestamp", Order = 5, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get;  }
}
