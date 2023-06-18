using Shibusa.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "symbol_meta", Schema = "public")]
internal sealed class SymbolMeta
{
    internal SymbolMeta(SymbolMetaData metaData)
    {
        Code = metaData.Code;
        Symbol = metaData.Symbol;
        Exchange = metaData.Exchange;
        Name = metaData.Name;
        Country = metaData.Country;
        Currency = metaData.Currency;
        Type = metaData.Type;
        Sector = metaData.Sector;
        Industry = metaData.Industry;
        LengthOfChart = metaData.LengthOfChart;
        LastClose = metaData.LastClose;
        LastVolume = metaData.LastVolume;
        LastDate = metaData.LastDate;
        HasOptions = metaData.HasOptions;
        HasDividends = metaData.HasDividends;
        HasSplits = metaData.HasSplits;
        Yield = metaData.Yield;
        FiscalYearEnd = metaData.FiscalYearEnd;
        MostRecentQuarter = metaData.MostRecentQuarter;
        LastUpdatedEntity = metaData.LastUpdatedEntity;
        LastUpdatedFinancials = metaData.LastUpdatedFinancials;
        LastUpdatedOptions = metaData.LastUpdatedOptions;
        UtcTimestamp = DateTime.UtcNow;
    }

    public SymbolMeta(
        string code,
        string? symbol,
        string? exchange,
        string? name,
        string? country,
        string? currency,
        string? type,
        string? sector,
        string? industry,
        int? lengthOfChart,
        decimal? lastClose,
        long? lastVolume,
        DateTime? lastDate,
        bool? hasOptions,
        bool? hasDividends,
        bool? hasSplits,
        decimal? yield,
        string? fiscalYearEnd,
        DateTime? mostRecentQuarter,
        DateTime? lastUpdatedEntity,
        DateTime? lastUpdatedFinancials,
        DateTime? lastUpdatedOptions,
        DateTime? utcTimestamp = null)
    {
        Code = code;
        Symbol = symbol;
        Exchange = exchange;
        Name = name;
        Country = country;
        Currency = currency;
        Type = type;
        Sector = sector;
        Industry = industry;
        LengthOfChart = lengthOfChart;
        LastClose = lastClose;
        LastVolume = lastVolume;
        LastDate = lastDate;
        HasOptions = hasOptions;
        HasDividends = hasDividends;
        HasSplits = hasSplits;
        Yield = yield;
        FiscalYearEnd = fiscalYearEnd;
        MostRecentQuarter = mostRecentQuarter;
        LastUpdatedEntity = lastUpdatedEntity;
        LastUpdatedFinancials = lastUpdatedFinancials;
        LastUpdatedOptions = lastUpdatedOptions;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("code", Order = 1, TypeName = "text", IsPartOfKey = true)]
    public string Code { get; }

    [ColumnWithKey("symbol", Order = 2, TypeName = "text", IsPartOfKey = false)]
    public string? Symbol { get; }

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

    [ColumnWithKey("sector", Order = 8, TypeName = "text", IsPartOfKey = false)]
    public string? Sector { get; }

    [ColumnWithKey("industry", Order = 9, TypeName = "text", IsPartOfKey = false)]
    public string? Industry { get; }

    [ColumnWithKey("length_of_chart", Order = 10, TypeName = "integer", IsPartOfKey = false)]
    public int? LengthOfChart { get; }

    [ColumnWithKey("last_close", Order = 11, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? LastClose { get; }

    [ColumnWithKey("last_volume", Order = 12, TypeName = "bigint", IsPartOfKey = false)]
    public long? LastVolume { get; }

    [ColumnWithKey("last_date", Order = 13, TypeName = "date", IsPartOfKey = false)]
    public DateTime? LastDate { get; }

    [ColumnWithKey("has_options", Order = 14, TypeName = "boolean", IsPartOfKey = false)]
    public bool? HasOptions { get; }

    [ColumnWithKey("has_dividends", Order = 15, TypeName = "boolean", IsPartOfKey = false)]
    public bool? HasDividends { get; }

    [ColumnWithKey("has_splits", Order = 16, TypeName = "boolean", IsPartOfKey = false)]
    public bool? HasSplits { get; }

    [ColumnWithKey("yield", Order = 17, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? Yield { get; }

    [ColumnWithKey("fiscal_year_end", Order = 18, TypeName = "text", IsPartOfKey = false)]
    public string? FiscalYearEnd { get; }

    [ColumnWithKey("most_recent_quarter", Order = 19, TypeName = "date", IsPartOfKey = false)]
    public DateTime? MostRecentQuarter { get; }

    [ColumnWithKey("last_updated_entity", Order = 20, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime? LastUpdatedEntity { get; }

    [ColumnWithKey("last_updated_financials", Order = 21, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime? LastUpdatedFinancials { get; }

    [ColumnWithKey("last_updated_options", Order = 22, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime? LastUpdatedOptions { get; }

    [ColumnWithKey("utc_timestamp", Order = 23, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
