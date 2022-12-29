using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "option_data", Schema = "public")]
internal class OptionData
{
    public OptionData(string symbol, string exchange,
        EodHistoricalData.Sdk.Models.Options.ContractCollection data)
    {
        Symbol = symbol;
        Exchange = exchange;
        ExpirationDate = data.ExpirationDate.ToDateTime(TimeOnly.MinValue);
        ImpliedVolatility = data.ImpliedVolatility;
        PutVolume = data.PutVolume;
        CallVolume = data.CallVolume;
        PutCallVolumeRatio = data.PutCallVolumeRatio;
        PutOpenInterest = data.PutOpenInterest;
        CallOpenInterest = data.CallOpenInterest;
        PutCallOpenInterestRatio = data.PutCallOpenInterestRatio;
        OptionsCount = data.OptionsCount;
        UtcTimestamp = DateTime.UtcNow;
    }

    public OptionData(
        string symbol,
        string exchange,
        DateTime expirationDate,
        double impliedVolatility,
        int putVolume,
        int callVolume,
        double putCallVolumeRatio,
        int putOpenInterest,
        int callOpenInterest,
        double putCallOpenInterestRatio,
        int optionsCount,
        DateTime utcTimestamp)
    {
        Symbol = symbol;
        Exchange = exchange;
        ExpirationDate = expirationDate;
        ImpliedVolatility = impliedVolatility;
        PutVolume = putVolume;
        CallVolume = callVolume;
        PutCallVolumeRatio = putCallVolumeRatio;
        PutOpenInterest = putOpenInterest;
        CallOpenInterest = callOpenInterest;
        PutCallOpenInterestRatio = putCallOpenInterestRatio;
        OptionsCount = optionsCount;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("symbol", Order = 1, TypeName = "text", IsPartOfKey = true)]
    public string Symbol { get; }

    [ColumnWithKey("exchange", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string Exchange { get; }

    [ColumnWithKey("expiration_date", Order = 3, TypeName = "date", IsPartOfKey = true)]
    public DateTime ExpirationDate { get; }

    [ColumnWithKey("implied_volatility", Order = 4, TypeName = "double precision", IsPartOfKey = false)]
    public double ImpliedVolatility { get; }

    [ColumnWithKey("put_volume", Order = 5, TypeName = "integer", IsPartOfKey = false)]
    public int PutVolume { get; }

    [ColumnWithKey("call_volume", Order = 6, TypeName = "integer", IsPartOfKey = false)]
    public int CallVolume { get; }

    [ColumnWithKey("put_call_volume_ratio", Order = 7, TypeName = "double precision", IsPartOfKey = false)]
    public double PutCallVolumeRatio { get; }

    [ColumnWithKey("put_open_interest", Order = 8, TypeName = "integer", IsPartOfKey = false)]
    public int PutOpenInterest { get; }

    [ColumnWithKey("call_open_interest", Order = 9, TypeName = "integer", IsPartOfKey = false)]
    public int CallOpenInterest { get; }

    [ColumnWithKey("put_call_open_interest_ratio", Order = 10, TypeName = "double precision", IsPartOfKey = false)]
    public double PutCallOpenInterestRatio { get; }

    [ColumnWithKey("options_count", Order = 11, TypeName = "integer", IsPartOfKey = false)]
    public int OptionsCount { get; }

    [ColumnWithKey("utc_timestamp", Order = 12, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
