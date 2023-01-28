using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "dividends", Schema = "public")]
internal class Dividend
{
    public Dividend(string symbol, string exchange, EodHistoricalData.Sdk.Models.Dividend dividend)
    {
        Symbol = symbol;
        Exchange = exchange;
        Date = dividend.Date.ToDateTime(TimeOnly.MinValue);
        Value = dividend.Value;
        UnadjustedValue = dividend.UnadjustedValue;
        Currency = dividend.Currency;
        DeclarationDate = dividend.DeclarationDate?.ToDateTime(TimeOnly.MinValue);
        RecordDate = dividend.RecordDate?.ToDateTime(TimeOnly.MinValue);
        PaymentDate = dividend.PaymentDate?.ToDateTime(TimeOnly.MinValue);
        Period = dividend.Period;
        UtcTimestamp = DateTime.UtcNow;
    }

    public Dividend(
        string symbol,
        string exchange,
        DateTime date,
        decimal? value,
        decimal? unadjustedValue,
        string? currency,
        DateTime? declarationDate,
        DateTime? recordDate,
        DateTime? paymentDate,
        string? period,
        DateTime? utcTimestamp = null)
    {
        Symbol = symbol;
        Exchange = exchange;
        Date = date;
        Value = value;
        UnadjustedValue = unadjustedValue;
        Currency = currency;
        DeclarationDate = declarationDate;
        RecordDate = recordDate;
        PaymentDate = paymentDate;
        Period = period;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("symbol", Order = 1, TypeName = "text", IsPartOfKey = true)]
    public string Symbol { get; }

    [ColumnWithKey("exchange", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string Exchange { get; }

    [ColumnWithKey("date", Order = 3, TypeName = "date", IsPartOfKey = true)]
    public DateTime Date { get; }

    [ColumnWithKey("value", Order = 4, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? Value { get; }

    [ColumnWithKey("unadjusted_value", Order = 5, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? UnadjustedValue { get; }

    [ColumnWithKey("currency", Order = 6, TypeName = "text", IsPartOfKey = false)]
    public string? Currency { get; }

    [ColumnWithKey("declaration_date", Order = 7, TypeName = "date", IsPartOfKey = false)]
    public DateTime? DeclarationDate { get; }

    [ColumnWithKey("record_date", Order = 8, TypeName = "date", IsPartOfKey = false)]
    public DateTime? RecordDate { get; }

    [ColumnWithKey("payment_date", Order = 9, TypeName = "date", IsPartOfKey = false)]
    public DateTime? PaymentDate { get; }

    [ColumnWithKey("period", Order = 10, TypeName = "text", IsPartOfKey = false)]
    public string? Period { get; }

    [ColumnWithKey("utc_timestamp", Order = 11, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
