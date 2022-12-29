using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "option_contracts", Schema = "public")]
internal class OptionContract
{
    public OptionContract(string symbol, string exchange,
        EodHistoricalData.Sdk.Models.Options.Contract contract)
    {
        Symbol = symbol;
        Exchange = exchange;
        ExpirationDate = contract.ExpirationDate.ToDateTime(TimeOnly.MinValue);
        OptionType = contract.Type;
        ContractName = contract.ContractName;
        ContractSize = contract.ContractSize;
        ContractPeriod = contract.ContractPeriod;
        Currency = contract.Currency;
        InTheMoney = contract.InTheMoney.Equals("true", StringComparison.InvariantCultureIgnoreCase);
        LastTradeDate = contract.LastTradeDateTime;
        Strike = contract.Strike;
        LastPrice = contract.LastPrice;
        Bid = contract.Bid;
        Ask = contract.Ask;
        Change = contract.Change;
        ChangePercent = contract.ChangePercent.GetValueOrDefault();
        Volume = contract.Volume.GetValueOrDefault();
        OpenInterest = contract.OpenInterest.GetValueOrDefault();
        ImpliedVolatility = contract.ImpliedVolatility;
        Delta = contract.Delta;
        Gamma = contract.Gamma;
        Theta = contract.Theta;
        Vega = contract.Vega;
        Rho = contract.Rho;
        Theoretical = contract.Theoretical;
        IntrinsicValue = contract.IntrinsicValue;
        TimeValue = contract.TimeValue;
        UpdatedAt = contract.UpdatedAt;
        UtcTimestamp = DateTime.UtcNow;
    }

    public OptionContract(
        string symbol,
        string exchange,
        DateTime expirationDate,
        string optionType,
        string contractName,
        string contractSize,
        string contractPeriod,
        string currency,
        bool inTheMoney,
        DateTime lastTradeDate,
        decimal strike,
        decimal lastPrice,
        decimal bid,
        decimal ask,
        decimal change,
        double changePercent,
        int volume,
        int openInterest,
        double impliedVolatility,
        double delta,
        double gamma,
        double theta,
        double vega,
        double rho,
        decimal theoretical,
        decimal intrinsicValue,
        decimal timeValue,
        DateTime updatedAt,
        DateTime utcTimestamp)
    {
        Symbol = symbol;
        Exchange = exchange;
        ExpirationDate = expirationDate;
        OptionType = optionType;
        ContractName = contractName;
        ContractSize = contractSize;
        ContractPeriod = contractPeriod;
        Currency = currency;
        InTheMoney = inTheMoney;
        LastTradeDate = lastTradeDate;
        Strike = strike;
        LastPrice = lastPrice;
        Bid = bid;
        Ask = ask;
        Change = change;
        ChangePercent = changePercent;
        Volume = volume;
        OpenInterest = openInterest;
        ImpliedVolatility = impliedVolatility;
        Delta = delta;
        Gamma = gamma;
        Theta = theta;
        Vega = vega;
        Rho = rho;
        Theoretical = theoretical;
        IntrinsicValue = intrinsicValue;
        TimeValue = timeValue;
        UpdatedAt = updatedAt;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("symbol", Order = 1, TypeName = "text", IsPartOfKey = true)]
    public string Symbol { get; }

    [ColumnWithKey("exchange", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string Exchange { get; }

    [ColumnWithKey("expiration_date", Order = 3, TypeName = "date", IsPartOfKey = true)]
    public DateTime ExpirationDate { get; }

    [ColumnWithKey("option_type", Order = 4, TypeName = "text", IsPartOfKey = true)]
    public string OptionType { get; }

    [ColumnWithKey("contract_name", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string ContractName { get; }

    [ColumnWithKey("contract_size", Order = 6, TypeName = "text", IsPartOfKey = false)]
    public string ContractSize { get; }

    [ColumnWithKey("contract_period", Order = 7, TypeName = "text", IsPartOfKey = false)]
    public string ContractPeriod { get; }

    [ColumnWithKey("currency", Order = 8, TypeName = "text", IsPartOfKey = false)]
    public string Currency { get; }

    [ColumnWithKey("in_the_money", Order = 9, TypeName = "boolean", IsPartOfKey = false)]
    public bool InTheMoney { get; }

    [ColumnWithKey("last_trade_date", Order = 10, TypeName = "date", IsPartOfKey = false)]
    public DateTime LastTradeDate { get; }

    [ColumnWithKey("strike", Order = 11, TypeName = "numeric", IsPartOfKey = false)]
    public decimal Strike { get; }

    [ColumnWithKey("last_price", Order = 12, TypeName = "numeric", IsPartOfKey = false)]
    public decimal LastPrice { get; }

    [ColumnWithKey("bid", Order = 13, TypeName = "numeric", IsPartOfKey = false)]
    public decimal Bid { get; }

    [ColumnWithKey("ask", Order = 14, TypeName = "numeric", IsPartOfKey = false)]
    public decimal Ask { get; }

    [ColumnWithKey("change", Order = 15, TypeName = "numeric", IsPartOfKey = false)]
    public decimal Change { get; }

    [ColumnWithKey("change_percent", Order = 16, TypeName = "double precision", IsPartOfKey = false)]
    public double ChangePercent { get; }

    [ColumnWithKey("volume", Order = 17, TypeName = "integer", IsPartOfKey = false)]
    public int Volume { get; }

    [ColumnWithKey("open_interest", Order = 18, TypeName = "integer", IsPartOfKey = false)]
    public int OpenInterest { get; }

    [ColumnWithKey("implied_volatility", Order = 19, TypeName = "double precision", IsPartOfKey = false)]
    public double ImpliedVolatility { get; }

    [ColumnWithKey("delta", Order = 20, TypeName = "double precision", IsPartOfKey = false)]
    public double Delta { get; }

    [ColumnWithKey("gamma", Order = 21, TypeName = "double precision", IsPartOfKey = false)]
    public double Gamma { get; }

    [ColumnWithKey("theta", Order = 22, TypeName = "double precision", IsPartOfKey = false)]
    public double Theta { get; }

    [ColumnWithKey("vega", Order = 23, TypeName = "double precision", IsPartOfKey = false)]
    public double Vega { get; }

    [ColumnWithKey("rho", Order = 24, TypeName = "double precision", IsPartOfKey = false)]
    public double Rho { get; }

    [ColumnWithKey("theoretical", Order = 25, TypeName = "numeric", IsPartOfKey = false)]
    public decimal Theoretical { get; }

    [ColumnWithKey("intrinsic_value", Order = 26, TypeName = "numeric", IsPartOfKey = false)]
    public decimal IntrinsicValue { get; }

    [ColumnWithKey("time_value", Order = 27, TypeName = "numeric", IsPartOfKey = false)]
    public decimal TimeValue { get; }

    [ColumnWithKey("updated_at", Order = 28, TypeName = "date", IsPartOfKey = false)]
    public DateTime UpdatedAt { get; }

    [ColumnWithKey("utc_timestamp", Order = 29, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
