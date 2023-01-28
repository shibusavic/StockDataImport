using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_cash_flows", Schema = "public")]
internal class CompanyCashFlow
{
    public CompanyCashFlow(Guid companyId, 
        string type,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.CashFlowItem cashFlowItem)
    {
        CompanyId = companyId;
        Type = type;
        Date = cashFlowItem.Date.GetValueOrDefault().ToDateTime(TimeOnly.MinValue);
        FilingDate = cashFlowItem.FilingDate.GetValueOrDefault().ToDateTime(TimeOnly.MinValue);
        CurrencySymbol = cashFlowItem.CurrencySymbol;
        Investments = cashFlowItem.Investments;
        ChangeToLiabilities = cashFlowItem.ChangeToLiabilities;
        TotalCashflowsFromInvestingActivities = cashFlowItem.TotalCashflowsFromInvestingActivities;
        NetBorrowings = cashFlowItem.NetBorrowings;
        TotalCashFromFinancingActivities = cashFlowItem.TotalCashFromFinancingActivities;
        ChangeToOperatingActivities = cashFlowItem.ChangeToOperatingActivities;
        NetIncome = cashFlowItem.NetIncome;
        ChangeInCash = cashFlowItem.ChangeInCash;
        BeginPeriodCashFlow = cashFlowItem.BeginPeriodCashFlow;
        EndPeriodCashFlow = cashFlowItem.EndPeriodCashFlow;
        TotalCashFromOperatingActivities = cashFlowItem.TotalCashFromOperatingActivities;
        IssuanceOfCapitalStock = cashFlowItem.IssuanceOfCapitalStock;
        Depreciation = cashFlowItem.Depreciation;
        OtherCashflowsFromInvestingActivities = cashFlowItem.OtherCashflowsFromInvestingActivities;
        DividendsPaid = cashFlowItem.DividendsPaid;
        ChangeToInventory = cashFlowItem.ChangeToInventory;
        ChangeToAccountReceivables = cashFlowItem.ChangeToAccountReceivables;
        SalePurchaseOfStock = cashFlowItem.SalePurchaseOfStock;
        OtherCashflowsFromFinancingActivities = cashFlowItem.OtherCashflowsFromFinancingActivities;
        ChangeToNetincome = cashFlowItem.ChangeToNetincome;
        CapitalExpenditures = cashFlowItem.CapitalExpenditures;
        ChangeReceivables = cashFlowItem.ChangeReceivables;
        CashFlowsOtherOperating = cashFlowItem.CashFlowsOtherOperating;
        ExchangeRateChanges = cashFlowItem.ExchangeRateChanges;
        CashAndCashEquivalentsChanges = cashFlowItem.CashAndCashEquivalentsChanges;
        ChangeInWorkingCapital = cashFlowItem.ChangeInWorkingCapital;
        OtherNonCashItems = cashFlowItem.OtherNonCashItems;
        FreeCashFlow = cashFlowItem.FreeCashFlow;
        CreatedTimestamp = DateTime.UtcNow;
        UtcTimestamp = DateTime.UtcNow;
    }

    [JsonConstructor]
    public CompanyCashFlow(
        Guid companyId,
        string type,
        DateTime date,
        DateTime? filingDate,
        string? currencySymbol,
        decimal? investments,
        decimal? changeToLiabilities,
        decimal? totalCashflowsFromInvestingActivities,
        decimal? netBorrowings,
        decimal? totalCashFromFinancingActivities,
        decimal? changeToOperatingActivities,
        decimal? netIncome,
        decimal? changeInCash,
        decimal? beginPeriodCashFlow,
        decimal? endPeriodCashFlow,
        decimal? totalCashFromOperatingActivities,
        decimal? issuanceOfCapitalStock,
        decimal? depreciation,
        decimal? otherCashflowsFromInvestingActivities,
        decimal? dividendsPaid,
        decimal? changeToInventory,
        decimal? changeToAccountReceivables,
        decimal? salePurchaseOfStock,
        decimal? otherCashflowsFromFinancingActivities,
        decimal? changeToNetincome,
        decimal? capitalExpenditures,
        decimal? changeReceivables,
        decimal? cashFlowsOtherOperating,
        decimal? exchangeRateChanges,
        decimal? cashAndCashEquivalentsChanges,
        decimal? changeInWorkingCapital,
        decimal? otherNonCashItems,
        decimal? freeCashFlow,
        DateTime? createdTimestamp = null,
        DateTime? utcTimestamp = null)
    {
        CompanyId = companyId;
        Type = type;
        Date = date;
        FilingDate = filingDate;
        CurrencySymbol = currencySymbol;
        Investments = investments;
        ChangeToLiabilities = changeToLiabilities;
        TotalCashflowsFromInvestingActivities = totalCashflowsFromInvestingActivities;
        NetBorrowings = netBorrowings;
        TotalCashFromFinancingActivities = totalCashFromFinancingActivities;
        ChangeToOperatingActivities = changeToOperatingActivities;
        NetIncome = netIncome;
        ChangeInCash = changeInCash;
        BeginPeriodCashFlow = beginPeriodCashFlow;
        EndPeriodCashFlow = endPeriodCashFlow;
        TotalCashFromOperatingActivities = totalCashFromOperatingActivities;
        IssuanceOfCapitalStock = issuanceOfCapitalStock;
        Depreciation = depreciation;
        OtherCashflowsFromInvestingActivities = otherCashflowsFromInvestingActivities;
        DividendsPaid = dividendsPaid;
        ChangeToInventory = changeToInventory;
        ChangeToAccountReceivables = changeToAccountReceivables;
        SalePurchaseOfStock = salePurchaseOfStock;
        OtherCashflowsFromFinancingActivities = otherCashflowsFromFinancingActivities;
        ChangeToNetincome = changeToNetincome;
        CapitalExpenditures = capitalExpenditures;
        ChangeReceivables = changeReceivables;
        CashFlowsOtherOperating = cashFlowsOtherOperating;
        ExchangeRateChanges = exchangeRateChanges;
        CashAndCashEquivalentsChanges = cashAndCashEquivalentsChanges;
        ChangeInWorkingCapital = changeInWorkingCapital;
        OtherNonCashItems = otherNonCashItems;
        FreeCashFlow = freeCashFlow;
        CreatedTimestamp = createdTimestamp ?? DateTime.UtcNow;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("type", Order = 2, TypeName = "text", IsPartOfKey = true)]
    public string Type { get; }

    [ColumnWithKey("date", Order = 3, TypeName = "date", IsPartOfKey = true)]
    public DateTime Date { get; }

    [ColumnWithKey("filing_date", Order = 4, TypeName = "date", IsPartOfKey = false)]
    public DateTime? FilingDate { get; }

    [ColumnWithKey("currency_symbol", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string? CurrencySymbol { get; }

    [ColumnWithKey("investments", Order = 6, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? Investments { get; }

    [ColumnWithKey("change_to_liabilities", Order = 7, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ChangeToLiabilities { get; }

    [ColumnWithKey("total_cashflows_from_investing_activities", Order = 8, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TotalCashflowsFromInvestingActivities { get; }

    [ColumnWithKey("net_borrowings", Order = 9, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NetBorrowings { get; }

    [ColumnWithKey("total_cash_from_financing_activities", Order = 10, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TotalCashFromFinancingActivities { get; }

    [ColumnWithKey("change_to_operating_activities", Order = 11, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ChangeToOperatingActivities { get; }

    [ColumnWithKey("net_income", Order = 12, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NetIncome { get; }

    [ColumnWithKey("change_in_cash", Order = 13, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ChangeInCash { get; }

    [ColumnWithKey("begin_period_cash_flow", Order = 14, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? BeginPeriodCashFlow { get; }

    [ColumnWithKey("end_period_cash_flow", Order = 15, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EndPeriodCashFlow { get; }

    [ColumnWithKey("total_cash_from_operating_activities", Order = 16, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TotalCashFromOperatingActivities { get; }

    [ColumnWithKey("issuance_of_capital_stock", Order = 17, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? IssuanceOfCapitalStock { get; }

    [ColumnWithKey("depreciation", Order = 18, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? Depreciation { get; }

    [ColumnWithKey("other_cashflows_from_investing_activities", Order = 19, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? OtherCashflowsFromInvestingActivities { get; }

    [ColumnWithKey("dividends_paid", Order = 20, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? DividendsPaid { get; }

    [ColumnWithKey("change_to_inventory", Order = 21, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ChangeToInventory { get; }

    [ColumnWithKey("change_to_account_receivables", Order = 22, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ChangeToAccountReceivables { get; }

    [ColumnWithKey("sale_purchase_of_stock", Order = 23, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? SalePurchaseOfStock { get; }

    [ColumnWithKey("other_cashflows_from_financing_activities", Order = 24, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? OtherCashflowsFromFinancingActivities { get; }

    [ColumnWithKey("change_to_netincome", Order = 25, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ChangeToNetincome { get; }

    [ColumnWithKey("capital_expenditures", Order = 26, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? CapitalExpenditures { get; }

    [ColumnWithKey("change_receivables", Order = 27, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ChangeReceivables { get; }

    [ColumnWithKey("cash_flows_other_operating", Order = 28, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? CashFlowsOtherOperating { get; }

    [ColumnWithKey("exchange_rate_changes", Order = 29, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ExchangeRateChanges { get; }

    [ColumnWithKey("cash_and_cash_equivalents_changes", Order = 30, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? CashAndCashEquivalentsChanges { get; }

    [ColumnWithKey("change_in_working_capital", Order = 31, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ChangeInWorkingCapital { get; }

    [ColumnWithKey("other_non_cash_items", Order = 32, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? OtherNonCashItems { get; }

    [ColumnWithKey("free_cash_flow", Order = 33, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? FreeCashFlow { get; }

    [ColumnWithKey("created_timestamp", Order = 34, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime CreatedTimestamp { get; }

    [ColumnWithKey("utc_timestamp", Order = 35, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
