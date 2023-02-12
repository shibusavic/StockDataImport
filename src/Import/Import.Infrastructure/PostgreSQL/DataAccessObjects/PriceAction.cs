using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;
using Shibusa.Extensions;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "price_actions", Schema = "public")]
internal class PriceAction
{
    public PriceAction(EodHistoricalData.Sdk.Models.Bulk.BulkPriceAction priceAction, string exchange)
    {
        Symbol = priceAction.Code ?? throw new ArgumentException($"Bulk price action does not have a code.");
        Start = priceAction.Date?.ToDateTime(TimeOnly.MinValue) ?? throw new ArgumentException($"Bulk price action for {Symbol} does not have a date.");
        Open = priceAction.Open ?? throw new ArgumentException($"Bulk price action for {Symbol} does not have an open.");
        High = priceAction.High ?? throw new ArgumentException($"Bulk price action for {Symbol} does not have a high.");
        Low = priceAction.Low ?? throw new ArgumentException($"Bulk price action for {Symbol} does not have a low.");
        Close = priceAction.Close ?? throw new ArgumentException($"Bulk price action for {Symbol} does not have a close.");
        Exchange = exchange;
        Finish = Start.EndOfDay();
        Volume = Convert.ToInt64(priceAction.Volume);
        UtcTimestamp = DateTime.UtcNow;
    }

    public PriceAction(string symbol, string exchange, EodHistoricalData.Sdk.Models.PriceAction priceAction)
    {
        Symbol = symbol;
        Exchange = exchange;
        Start = priceAction.Date.ToDateTime(TimeOnly.MinValue);
        Finish = Start.EndOfDay();
        Open = priceAction.Open;
        High = priceAction.High;
        Low = priceAction.Low;
        Close = priceAction.Close;
        Volume = Convert.ToInt64(priceAction.Volume);
        UtcTimestamp = DateTime.UtcNow;
    }

    public PriceAction(
    string symbol,
    string exchange,
    DateTime start,
    DateTime finish,
    decimal open,
    decimal high,
    decimal low,
    decimal close,
    int volume,
    DateTime? utcTimestamp = null)
    {
        Symbol = symbol;
        Exchange = exchange;
        Start = start;
        Finish = finish;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }

    [ColumnWithKey("symbol", Order = 1, TypeName = "text", IsPartOfKey = true)]
    public string Symbol { get; }

    [ColumnWithKey("exchange", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string Exchange { get; }

    [ColumnWithKey("start", Order = 3, TypeName = "timestamp without time zone", IsPartOfKey = true)]
    public DateTime Start { get; }

    [ColumnWithKey("finish", Order = 4, TypeName = "timestamp without time zone", IsPartOfKey = true)]
    public DateTime Finish { get; }

    [ColumnWithKey("open", Order = 5, TypeName = "numeric", IsPartOfKey = false)]
    public decimal Open { get; }

    [ColumnWithKey("high", Order = 6, TypeName = "numeric", IsPartOfKey = false)]
    public decimal High { get; }

    [ColumnWithKey("low", Order = 7, TypeName = "numeric", IsPartOfKey = false)]
    public decimal Low { get; }

    [ColumnWithKey("close", Order = 8, TypeName = "numeric", IsPartOfKey = false)]
    public decimal Close { get; }

    [ColumnWithKey("volume", Order = 9, TypeName = "bigint", IsPartOfKey = false)]
    public long Volume { get; }

    [ColumnWithKey("utc_timestamp", Order = 10, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
