using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "options", Schema = "public")]
internal class Options
{
    public Options(EodHistoricalData.Sdk.Models.Options.OptionsCollection options)
    {
        Symbol = options.Code ??
            throw new ArgumentException($"{nameof(options)} has no {nameof(Symbol)}"); 
        Exchange = options.Exchange ?? throw new ArgumentException($"{nameof(options)} has no {nameof(Exchange)}");
        LastTradeDate = options.LastTradeDate?.ToDateTime(TimeOnly.MinValue) ??
            throw new ArgumentException($"{nameof(options)} has no {nameof(LastTradeDate)}");
        LastTradePrice = options.LastTradePrice;
        CreatedTimestamp = DateTime.UtcNow;
        UtcTimestamp = DateTime.UtcNow;
    }

    public Options(
        string symbol,
        string exchange,
        DateTime lastTradeDate,
        decimal? lastTradePrice,
        DateTime? createdTimestamp = null,
        DateTime? utcTimestamp = null)
    {
        Symbol = symbol;
        Exchange = exchange;
        LastTradeDate = lastTradeDate;
        LastTradePrice = lastTradePrice;
        CreatedTimestamp = createdTimestamp ?? DateTime.UtcNow;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }

    [ColumnWithKey("symbol", Order = 1, TypeName = "text", IsPartOfKey = true)]
    public string Symbol { get; }

    [ColumnWithKey("exchange", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string Exchange { get; }

    [ColumnWithKey("last_trade_date", Order = 3, TypeName = "timestamp without time zone", IsPartOfKey = true)]
    public DateTime LastTradeDate { get; }

    [ColumnWithKey("last_trade_price", Order = 4, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? LastTradePrice { get; }

    [ColumnWithKey("created_timestamp", Order = 5, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime CreatedTimestamp { get; }

    [ColumnWithKey("utc_timestamp", Order = 6, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
