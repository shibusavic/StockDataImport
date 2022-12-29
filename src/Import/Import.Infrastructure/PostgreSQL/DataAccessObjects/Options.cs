using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "options", Schema = "public")]
internal class Options
{
    public Options(
        EodHistoricalData.Sdk.Models.Options.OptionsCollection options)
    {
        Symbol = options.Code;
        Exchange = options.Exchange;
        LastTradeDate = options.LastTradeDate.ToDateTime(TimeOnly.MinValue);
        LastTradePrice = options.LastTradePrice;
        UtcTimestamp = DateTime.UtcNow;
    }

    public Options(
        string symbol,
        string exchange,
        object lastTradeDate,
        decimal lastTradePrice,
        DateTime utcTimestamp)
    {
        Symbol = symbol;
        Exchange = exchange;
        LastTradeDate = lastTradeDate;
        LastTradePrice = lastTradePrice;
        UtcTimestamp = utcTimestamp;
    }

    [ColumnWithKey("symbol", Order = 1, TypeName = "text", IsPartOfKey = true)]
    public string Symbol { get; }

    [ColumnWithKey("exchange", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string Exchange { get; }

    [ColumnWithKey("last_trade_date", Order = 3, TypeName = "timestamp without time zone", IsPartOfKey = true)]
    public object LastTradeDate { get; }

    [ColumnWithKey("last_trade_price", Order = 4, TypeName = "numeric", IsPartOfKey = false)]
    public decimal LastTradePrice { get; }

    [ColumnWithKey("utc_timestamp", Order = 5, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
