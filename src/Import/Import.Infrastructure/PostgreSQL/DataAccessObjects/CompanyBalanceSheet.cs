using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_balance_sheets", Schema = "public")]
internal class CompanyBalanceSheet
{
    public CompanyBalanceSheet(Guid companyId,
        string type,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.BalanceSheetItem balanceSheetItem)
    {
        CompanyId = companyId;
        DateCaptured = DateTime.UtcNow;
        Type = type;
        Date = balanceSheetItem.Date.GetValueOrDefault().ToDateTime(TimeOnly.MinValue);
        FilingDate = balanceSheetItem.FilingDate.GetValueOrDefault().ToDateTime(TimeOnly.MinValue);
        CurrencySymbol = balanceSheetItem.CurrencySymbol;
        TotalAssets = balanceSheetItem.TotalAssets;
        IntangibleAssets = balanceSheetItem.IntangibleAssets;
        EarningAssets = balanceSheetItem.EarningAssets;
        OtherCurrentAssets = balanceSheetItem.OtherCurrentAssets;
        TotalLiab = balanceSheetItem.TotalLiab;
        TotalStockholderEquity = balanceSheetItem.TotalStockholderEquity;
        DeferredLongTermLiab = balanceSheetItem.DeferredLongTermLiab;
        OtherCurrentLiab = balanceSheetItem.OtherCurrentLiab;
        CommonStock = balanceSheetItem.CommonStock;
        CapitalStock = balanceSheetItem.CapitalStock;
        RetainedEarnings = balanceSheetItem.RetainedEarnings;
        OtherLiab = balanceSheetItem.OtherLiab;
        GoodWill = balanceSheetItem.GoodWill;
        OtherAssets = balanceSheetItem.OtherAssets;
        Cash = balanceSheetItem.Cash;
        CashAndEquivalents = balanceSheetItem.CashAndEquivalents;
        TotalCurrentLiabilities = balanceSheetItem.TotalCurrentLiabilities;
        CurrentDeferredRevenue = balanceSheetItem.CurrentDeferredRevenue;
        NetDebt = balanceSheetItem.NetDebt;
        ShortTermDebt = balanceSheetItem.ShortTermDebt;
        ShortLongTermDebt = balanceSheetItem.ShortLongTermDebt;
        ShortLongTermDebtTotal = balanceSheetItem.ShortLongTermDebtTotal;
        OtherStockholderEquity = balanceSheetItem.OtherStockholderEquity;
        PropertyPlantEquipment = balanceSheetItem.PropertyPlantEquipment;
        TotalCurrentAssets = balanceSheetItem.TotalCurrentAssets;
        LongTermInvestments = balanceSheetItem.LongTermInvestments;
        NetTangibleAssets = balanceSheetItem.NetTangibleAssets;
        ShortTermInvestments = balanceSheetItem.ShortTermInvestments;
        NetReceivables = balanceSheetItem.NetReceivables;
        LongTermDebt = balanceSheetItem.LongTermDebt;
        Inventory = balanceSheetItem.Inventory;
        AccountsPayable = balanceSheetItem.AccountsPayable;
        TotalPermanentEquity = balanceSheetItem.TotalPermanentEquity;
        NoncontrollingInterestInConsolidatedEntity = balanceSheetItem.NoncontrollingInterestInConsolidatedEntity;
        TemporaryEquityRedeemableNoncontrollingInterests = balanceSheetItem.TemporaryEquityRedeemableNoncontrollingInterests;
        AccumulatedOtherComprehensiveIncome = balanceSheetItem.AccumulatedOtherComprehensiveIncome;
        AdditionalPaidInCapital = balanceSheetItem.AdditionalPaidInCapital;
        CommonStockTotalEquity = balanceSheetItem.CommonStockTotalEquity;
        PreferredStockTotalEquity = balanceSheetItem.PreferredStockTotalEquity;
        RetainedEarningsTotalEquity = balanceSheetItem.RetainedEarningsTotalEquity;
        TreasuryStock = balanceSheetItem.TreasuryStock;
        AccumulatedAmortization = balanceSheetItem.AccumulatedAmortization;
        NonCurrrentAssetsOther = balanceSheetItem.NonCurrrentAssetsOther;
        DeferredLongTermAssetCharges = balanceSheetItem.DeferredLongTermAssetCharges;
        NonCurrentAssetsTotal = balanceSheetItem.NonCurrentAssetsTotal;
        CapitalLeaseObligations = balanceSheetItem.CapitalLeaseObligations;
        LongTermDebtTotal = balanceSheetItem.LongTermDebtTotal;
        NonCurrentLiabilitiesOther = balanceSheetItem.NonCurrentLiabilitiesOther;
        NonCurrentLiabilitiesTotal = balanceSheetItem.NonCurrentLiabilitiesTotal;
        NegativeGoodwill = balanceSheetItem.NegativeGoodwill;
        Warrants = balanceSheetItem.Warrants;
        PreferredStockRedeemable = balanceSheetItem.PreferredStockRedeemable;
        CapitalSurpluses = balanceSheetItem.CapitalSurpluse;
        LiabilitiesAndStockholdersEquity = balanceSheetItem.LiabilitiesAndStockholdersEquity;
        CashAndShortTermInvestments = balanceSheetItem.CashAndShortTermInvestments;
        PropertyPlantAndEquipmentGross = balanceSheetItem.PropertyPlantAndEquipmentGross;
        PropertyPlantAndEquipmentNet = balanceSheetItem.PropertyPlantAndEquipmentNet;
        AccumulatedDepreciation = balanceSheetItem.AccumulatedDepreciation;
        NetWorkingCapital = balanceSheetItem.NetWorkingCapital;
        NetInvestedCapital = balanceSheetItem.NetInvestedCapital;
        CommonStockSharesOutstanding = balanceSheetItem.CommonStockSharesOutstanding;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyBalanceSheet(
        Guid companyId,
        DateTime dateCaptured,
        string type,
        DateTime date,
        DateTime filingDate,
        string currencySymbol,
        decimal? totalAssets,
        decimal? intangibleAssets,
        decimal? earningAssets,
        decimal? otherCurrentAssets,
        decimal? totalLiab,
        decimal? totalStockholderEquity,
        decimal? deferredLongTermLiab,
        decimal? otherCurrentLiab,
        decimal? commonStock,
        decimal? capitalStock,
        decimal? retainedEarnings,
        decimal? otherLiab,
        decimal? goodWill,
        decimal? otherAssets,
        decimal? cash,
        decimal? cashAndEquivalents,
        decimal? totalCurrentLiabilities,
        decimal? currentDeferredRevenue,
        decimal? netDebt,
        decimal? shortTermDebt,
        decimal? shortLongTermDebt,
        decimal? shortLongTermDebtTotal,
        decimal? otherStockholderEquity,
        decimal? propertyPlantEquipment,
        decimal? totalCurrentAssets,
        decimal? longTermInvestments,
        decimal? netTangibleAssets,
        decimal? shortTermInvestments,
        decimal? netReceivables,
        decimal? longTermDebt,
        decimal? inventory,
        decimal? accountsPayable,
        decimal? totalPermanentEquity,
        decimal? noncontrollingInterestInConsolidatedEntity,
        decimal? temporaryEquityRedeemableNoncontrollingInterests,
        decimal? accumulatedOtherComprehensiveIncome,
        decimal? additionalPaidInCapital,
        decimal? commonStockTotalEquity,
        decimal? preferredStockTotalEquity,
        decimal? retainedEarningsTotalEquity,
        decimal? treasuryStock,
        decimal? accumulatedAmortization,
        decimal? nonCurrrentAssetsOther,
        decimal? deferredLongTermAssetCharges,
        decimal? nonCurrentAssetsTotal,
        decimal? capitalLeaseObligations,
        decimal? longTermDebtTotal,
        decimal? nonCurrentLiabilitiesOther,
        decimal? nonCurrentLiabilitiesTotal,
        decimal? negativeGoodwill,
        decimal? warrants,
        decimal? preferredStockRedeemable,
        decimal? capitalSurpluse,
        decimal? liabilitiesAndStockholdersEquity,
        decimal? cashAndShortTermInvestments,
        decimal? propertyPlantAndEquipmentGross,
        decimal? propertyPlantAndEquipmentNet,
        decimal? accumulatedDepreciation,
        decimal? netWorkingCapital,
        decimal? netInvestedCapital,
        decimal? commonStockSharesOutstanding,
        DateTime utcTimestamp)
    {
        CompanyId = companyId;
        DateCaptured = dateCaptured;
        Type = type;
        Date = date;
        FilingDate = filingDate;
        CurrencySymbol = currencySymbol;
        TotalAssets = totalAssets;
        IntangibleAssets = intangibleAssets;
        EarningAssets = earningAssets;
        OtherCurrentAssets = otherCurrentAssets;
        TotalLiab = totalLiab;
        TotalStockholderEquity = totalStockholderEquity;
        DeferredLongTermLiab = deferredLongTermLiab;
        OtherCurrentLiab = otherCurrentLiab;
        CommonStock = commonStock;
        CapitalStock = capitalStock;
        RetainedEarnings = retainedEarnings;
        OtherLiab = otherLiab;
        GoodWill = goodWill;
        OtherAssets = otherAssets;
        Cash = cash;
        CashAndEquivalents = cashAndEquivalents;
        TotalCurrentLiabilities = totalCurrentLiabilities;
        CurrentDeferredRevenue = currentDeferredRevenue;
        NetDebt = netDebt;
        ShortTermDebt = shortTermDebt;
        ShortLongTermDebt = shortLongTermDebt;
        ShortLongTermDebtTotal = shortLongTermDebtTotal;
        OtherStockholderEquity = otherStockholderEquity;
        PropertyPlantEquipment = propertyPlantEquipment;
        TotalCurrentAssets = totalCurrentAssets;
        LongTermInvestments = longTermInvestments;
        NetTangibleAssets = netTangibleAssets;
        ShortTermInvestments = shortTermInvestments;
        NetReceivables = netReceivables;
        LongTermDebt = longTermDebt;
        Inventory = inventory;
        AccountsPayable = accountsPayable;
        TotalPermanentEquity = totalPermanentEquity;
        NoncontrollingInterestInConsolidatedEntity = noncontrollingInterestInConsolidatedEntity;
        TemporaryEquityRedeemableNoncontrollingInterests = temporaryEquityRedeemableNoncontrollingInterests;
        AccumulatedOtherComprehensiveIncome = accumulatedOtherComprehensiveIncome;
        AdditionalPaidInCapital = additionalPaidInCapital;
        CommonStockTotalEquity = commonStockTotalEquity;
        PreferredStockTotalEquity = preferredStockTotalEquity;
        RetainedEarningsTotalEquity = retainedEarningsTotalEquity;
        TreasuryStock = treasuryStock;
        AccumulatedAmortization = accumulatedAmortization;
        NonCurrrentAssetsOther = nonCurrrentAssetsOther;
        DeferredLongTermAssetCharges = deferredLongTermAssetCharges;
        NonCurrentAssetsTotal = nonCurrentAssetsTotal;
        CapitalLeaseObligations = capitalLeaseObligations;
        LongTermDebtTotal = longTermDebtTotal;
        NonCurrentLiabilitiesOther = nonCurrentLiabilitiesOther;
        NonCurrentLiabilitiesTotal = nonCurrentLiabilitiesTotal;
        NegativeGoodwill = negativeGoodwill;
        Warrants = warrants;
        PreferredStockRedeemable = preferredStockRedeemable;
        CapitalSurpluses = capitalSurpluse;
        LiabilitiesAndStockholdersEquity = liabilitiesAndStockholdersEquity;
        CashAndShortTermInvestments = cashAndShortTermInvestments;
        PropertyPlantAndEquipmentGross = propertyPlantAndEquipmentGross;
        PropertyPlantAndEquipmentNet = propertyPlantAndEquipmentNet;
        AccumulatedDepreciation = accumulatedDepreciation;
        NetWorkingCapital = netWorkingCapital;
        NetInvestedCapital = netInvestedCapital;
        CommonStockSharesOutstanding = commonStockSharesOutstanding;
        UtcTimestamp = utcTimestamp;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get;  }

    [ColumnWithKey("date_captured", Order = 2, TypeName = "date", IsPartOfKey = false)]
    public DateTime DateCaptured { get;  }

    [ColumnWithKey("type", Order = 3, TypeName = "text", IsPartOfKey = true)]
    public string Type { get;  }

    [ColumnWithKey("date", Order = 4, TypeName = "date", IsPartOfKey = true)]
    public DateTime Date { get;  }

    [ColumnWithKey("filing_date", Order = 5, TypeName = "date", IsPartOfKey = false)]
    public DateTime FilingDate { get;  }

    [ColumnWithKey("currency_symbol", Order = 6, TypeName = "text", IsPartOfKey = false)]
    public string CurrencySymbol { get;  }

    [ColumnWithKey("total_assets", Order = 7, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TotalAssets { get;  }

    [ColumnWithKey("intangible_assets", Order = 8, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? IntangibleAssets { get;  }

    [ColumnWithKey("earning_assets", Order = 9, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EarningAssets { get;  }

    [ColumnWithKey("other_current_assets", Order = 10, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? OtherCurrentAssets { get;  }

    [ColumnWithKey("total_liab", Order = 11, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TotalLiab { get;  }

    [ColumnWithKey("total_stockholder_equity", Order = 12, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TotalStockholderEquity { get;  }

    [ColumnWithKey("deferred_long_term_liab", Order = 13, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? DeferredLongTermLiab { get;  }

    [ColumnWithKey("other_current_liab", Order = 14, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? OtherCurrentLiab { get;  }

    [ColumnWithKey("common_stock", Order = 15, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? CommonStock { get;  }

    [ColumnWithKey("capital_stock", Order = 16, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? CapitalStock { get;  }

    [ColumnWithKey("retained_earnings", Order = 17, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? RetainedEarnings { get;  }

    [ColumnWithKey("other_liab", Order = 18, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? OtherLiab { get;  }

    [ColumnWithKey("good_will", Order = 19, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? GoodWill { get;  }

    [ColumnWithKey("other_assets", Order = 20, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? OtherAssets { get;  }

    [ColumnWithKey("cash", Order = 21, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? Cash { get;  }

    [ColumnWithKey("cash_and_equivalents", Order = 22, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? CashAndEquivalents { get;  }

    [ColumnWithKey("total_current_liabilities", Order = 23, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TotalCurrentLiabilities { get;  }

    [ColumnWithKey("current_deferred_revenue", Order = 24, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? CurrentDeferredRevenue { get;  }

    [ColumnWithKey("net_debt", Order = 25, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NetDebt { get;  }

    [ColumnWithKey("short_term_debt", Order = 26, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ShortTermDebt { get;  }

    [ColumnWithKey("short_long_term_debt", Order = 27, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ShortLongTermDebt { get;  }

    [ColumnWithKey("short_long_term_debt_total", Order = 28, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ShortLongTermDebtTotal { get;  }

    [ColumnWithKey("other_stockholder_equity", Order = 29, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? OtherStockholderEquity { get;  }

    [ColumnWithKey("property_plant_equipment", Order = 30, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? PropertyPlantEquipment { get;  }

    [ColumnWithKey("total_current_assets", Order = 31, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TotalCurrentAssets { get;  }

    [ColumnWithKey("long_term_investments", Order = 32, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? LongTermInvestments { get;  }

    [ColumnWithKey("net_tangible_assets", Order = 33, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NetTangibleAssets { get;  }

    [ColumnWithKey("short_term_investments", Order = 34, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ShortTermInvestments { get;  }

    [ColumnWithKey("net_receivables", Order = 35, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NetReceivables { get;  }

    [ColumnWithKey("long_term_debt", Order = 36, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? LongTermDebt { get;  }

    [ColumnWithKey("inventory", Order = 37, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? Inventory { get;  }

    [ColumnWithKey("accounts_payable", Order = 38, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? AccountsPayable { get;  }

    [ColumnWithKey("total_permanent_equity", Order = 39, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TotalPermanentEquity { get;  }

    [ColumnWithKey("noncontrolling_interest_in_consolidated_entity", Order = 40, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NoncontrollingInterestInConsolidatedEntity { get;  }

    [ColumnWithKey("temporary_equity_redeemable_noncontrolling_interests", Order = 41, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TemporaryEquityRedeemableNoncontrollingInterests { get;  }

    [ColumnWithKey("accumulated_other_comprehensive_income", Order = 42, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? AccumulatedOtherComprehensiveIncome { get;  }

    [ColumnWithKey("additional_paid_in_capital", Order = 43, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? AdditionalPaidInCapital { get;  }

    [ColumnWithKey("common_stock_total_equity", Order = 44, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? CommonStockTotalEquity { get;  }

    [ColumnWithKey("preferred_stock_total_equity", Order = 45, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? PreferredStockTotalEquity { get;  }

    [ColumnWithKey("retained_earnings_total_equity", Order = 46, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? RetainedEarningsTotalEquity { get;  }

    [ColumnWithKey("treasury_stock", Order = 47, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TreasuryStock { get;  }

    [ColumnWithKey("accumulated_amortization", Order = 48, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? AccumulatedAmortization { get;  }

    [ColumnWithKey("non_currrent_assets_other", Order = 49, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NonCurrrentAssetsOther { get;  }

    [ColumnWithKey("deferred_long_term_asset_charges", Order = 50, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? DeferredLongTermAssetCharges { get;  }

    [ColumnWithKey("non_current_assets_total", Order = 51, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NonCurrentAssetsTotal { get;  }

    [ColumnWithKey("capital_lease_obligations", Order = 52, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? CapitalLeaseObligations { get;  }

    [ColumnWithKey("long_term_debt_total", Order = 53, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? LongTermDebtTotal { get;  }

    [ColumnWithKey("non_current_liabilities_other", Order = 54, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NonCurrentLiabilitiesOther { get;  }

    [ColumnWithKey("non_current_liabilities_total", Order = 55, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NonCurrentLiabilitiesTotal { get;  }

    [ColumnWithKey("negative_goodwill", Order = 56, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NegativeGoodwill { get;  }

    [ColumnWithKey("warrants", Order = 57, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? Warrants { get;  }

    [ColumnWithKey("preferred_stock_redeemable", Order = 58, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? PreferredStockRedeemable { get;  }

    [ColumnWithKey("capital_surpluses", Order = 59, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? CapitalSurpluses { get;  }

    [ColumnWithKey("liabilities_and_stockholders_equity", Order = 60, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? LiabilitiesAndStockholdersEquity { get;  }

    [ColumnWithKey("cash_and_short_term_investments", Order = 61, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? CashAndShortTermInvestments { get;  }

    [ColumnWithKey("property_plant_and_equipment_gross", Order = 62, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? PropertyPlantAndEquipmentGross { get;  }

    [ColumnWithKey("property_plant_and_equipment_net", Order = 63, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? PropertyPlantAndEquipmentNet { get;  }

    [ColumnWithKey("accumulated_depreciation", Order = 64, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? AccumulatedDepreciation { get;  }

    [ColumnWithKey("net_working_capital", Order = 65, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NetWorkingCapital { get;  }

    [ColumnWithKey("net_invested_capital", Order = 66, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NetInvestedCapital { get;  }

    [ColumnWithKey("common_stock_shares_outstanding", Order = 67, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? CommonStockSharesOutstanding { get;  }

    [ColumnWithKey("utc_timestamp", Order = 68, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get;  }
}
