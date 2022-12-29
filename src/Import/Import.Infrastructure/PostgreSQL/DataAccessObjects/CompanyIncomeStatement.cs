using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_income_statements", Schema = "public")]
internal class CompanyIncomeStatement
{
    public CompanyIncomeStatement(Guid companyId,
        string type,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.IncomeStatementItem incomeStatementItem)
    {
        CompanyId = companyId;
        DateCaptured = DateTime.UtcNow;
        Type = type;
        Date = incomeStatementItem.Date.GetValueOrDefault().ToDateTime(TimeOnly.MinValue);
        FilingDate = incomeStatementItem.FilingDate.GetValueOrDefault().ToDateTime(TimeOnly.MinValue);
        CurrencySymbol = incomeStatementItem.CurrencySymbol;
        ResearchDevelopment = incomeStatementItem.ResearchDevelopment;
        EffectOfAccountingCharges = incomeStatementItem.EffectOfAccountingCharges;
        IncomeBeforeTax = incomeStatementItem.IncomeBeforeTax;
        MinorityInterest = incomeStatementItem.MinorityInterest;
        NetIncome = incomeStatementItem.NetIncome;
        SellingGeneralAdministrative = incomeStatementItem.SellingGeneralAdministrative;
        SellingAndMarketingExpenses = incomeStatementItem.SellingAndMarketingExpenses;
        GrossProfit = incomeStatementItem.GrossProfit;
        ReconciledDepreciation = incomeStatementItem.ReconciledDepreciation;
        Ebit = incomeStatementItem.Ebit;
        Ebitda = incomeStatementItem.Ebitda;
        DepreciationAndAmortization = incomeStatementItem.DepreciationAndAmortization;
        NonOperatingIncomeNetOther = incomeStatementItem.NonOperatingIncomeNetOther;
        OperatingIncome = incomeStatementItem.OperatingIncome;
        OtherOperatingExpenses = incomeStatementItem.OtherOperatingExpenses;
        InterestExpense = incomeStatementItem.InterestExpense;
        TaxProvision = incomeStatementItem.TaxProvision;
        InterestIncome = incomeStatementItem.InterestIncome;
        NetInterestIncome = incomeStatementItem.NetInterestIncome;
        ExtraordinaryItems = incomeStatementItem.ExtraordinaryItems;
        NonRecurring = incomeStatementItem.NonRecurring;
        OtherItems = incomeStatementItem.OtherItems;
        IncomeTaxExpense = incomeStatementItem.IncomeTaxExpense;
        TotalRevenue = incomeStatementItem.TotalRevenue;
        TotalOperatingExpenses = incomeStatementItem.TotalOperatingExpenses;
        CostOfRevenue = incomeStatementItem.CostOfRevenue;
        TotalOtherIncomeExpenseNet = incomeStatementItem.TotalOtherIncomeExpenseNet;
        DiscontinuedOperations = incomeStatementItem.DiscontinuedOperations;
        NetIncomeFromContinuingOps = incomeStatementItem.NetIncomeFromContinuingOps;
        NetIncomeApplicableToCommonShares = incomeStatementItem.NetIncomeApplicableToCommonShares;
        PreferredStockAndOtherAdjustments = incomeStatementItem.PreferredStockAndOtherAdjustments;
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyIncomeStatement(
        Guid companyId,
        DateTime dateCaptured,
        string type,
        DateTime date,
        DateTime filingDate,
        string currencySymbol,
        decimal? researchDevelopment,
        decimal? effectOfAccountingCharges,
        decimal? incomeBeforeTax,
        decimal? minorityInterest,
        decimal? netIncome,
        decimal? sellingGeneralAdministrative,
        decimal? sellingAndMarketingExpenses,
        decimal? grossProfit,
        decimal? reconciledDepreciation,
        decimal? ebit,
        decimal? ebitda,
        decimal? depreciationAndAmortization,
        decimal? nonOperatingIncomeNetOther,
        decimal? operatingIncome,
        decimal? otherOperatingExpenses,
        decimal? interestExpense,
        decimal? taxProvision,
        decimal? interestIncome,
        decimal? netInterestIncome,
        decimal? extraordinaryItems,
        decimal? nonRecurring,
        decimal? otherItems,
        decimal? incomeTaxExpense,
        decimal? totalRevenue,
        decimal? totalOperatingExpenses,
        decimal? costOfRevenue,
        decimal? totalOtherIncomeExpenseNet,
        decimal? discontinuedOperations,
        decimal? netIncomeFromContinuingOps,
        decimal? netIncomeApplicableToCommonShares,
        decimal? preferredStockAndOtherAdjustments,
        DateTime utcTimestamp)
    {
        CompanyId = companyId;
        DateCaptured = dateCaptured;
        Type = type;
        Date = date;
        FilingDate = filingDate;
        CurrencySymbol = currencySymbol;
        ResearchDevelopment = researchDevelopment;
        EffectOfAccountingCharges = effectOfAccountingCharges;
        IncomeBeforeTax = incomeBeforeTax;
        MinorityInterest = minorityInterest;
        NetIncome = netIncome;
        SellingGeneralAdministrative = sellingGeneralAdministrative;
        SellingAndMarketingExpenses = sellingAndMarketingExpenses;
        GrossProfit = grossProfit;
        ReconciledDepreciation = reconciledDepreciation;
        Ebit = ebit;
        Ebitda = ebitda;
        DepreciationAndAmortization = depreciationAndAmortization;
        NonOperatingIncomeNetOther = nonOperatingIncomeNetOther;
        OperatingIncome = operatingIncome;
        OtherOperatingExpenses = otherOperatingExpenses;
        InterestExpense = interestExpense;
        TaxProvision = taxProvision;
        InterestIncome = interestIncome;
        NetInterestIncome = netInterestIncome;
        ExtraordinaryItems = extraordinaryItems;
        NonRecurring = nonRecurring;
        OtherItems = otherItems;
        IncomeTaxExpense = incomeTaxExpense;
        TotalRevenue = totalRevenue;
        TotalOperatingExpenses = totalOperatingExpenses;
        CostOfRevenue = costOfRevenue;
        TotalOtherIncomeExpenseNet = totalOtherIncomeExpenseNet;
        DiscontinuedOperations = discontinuedOperations;
        NetIncomeFromContinuingOps = netIncomeFromContinuingOps;
        NetIncomeApplicableToCommonShares = netIncomeApplicableToCommonShares;
        PreferredStockAndOtherAdjustments = preferredStockAndOtherAdjustments;
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

    [ColumnWithKey("research_development", Order = 7, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ResearchDevelopment { get;  }

    [ColumnWithKey("effect_of_accounting_charges", Order = 8, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EffectOfAccountingCharges { get;  }

    [ColumnWithKey("income_before_tax", Order = 9, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? IncomeBeforeTax { get;  }

    [ColumnWithKey("minority_interest", Order = 10, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? MinorityInterest { get;  }

    [ColumnWithKey("net_income", Order = 11, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NetIncome { get;  }

    [ColumnWithKey("selling_general_administrative", Order = 12, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? SellingGeneralAdministrative { get;  }

    [ColumnWithKey("selling_and_marketing_expenses", Order = 13, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? SellingAndMarketingExpenses { get;  }

    [ColumnWithKey("gross_profit", Order = 14, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? GrossProfit { get;  }

    [ColumnWithKey("reconciled_depreciation", Order = 15, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ReconciledDepreciation { get;  }

    [ColumnWithKey("ebit", Order = 16, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? Ebit { get;  }

    [ColumnWithKey("ebitda", Order = 17, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? Ebitda { get;  }

    [ColumnWithKey("depreciation_and_amortization", Order = 18, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? DepreciationAndAmortization { get;  }

    [ColumnWithKey("non_operating_income_net_other", Order = 19, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NonOperatingIncomeNetOther { get;  }

    [ColumnWithKey("operating_income", Order = 20, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? OperatingIncome { get;  }

    [ColumnWithKey("other_operating_expenses", Order = 21, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? OtherOperatingExpenses { get;  }

    [ColumnWithKey("interest_expense", Order = 22, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? InterestExpense { get;  }

    [ColumnWithKey("tax_provision", Order = 23, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TaxProvision { get;  }

    [ColumnWithKey("interest_income", Order = 24, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? InterestIncome { get;  }

    [ColumnWithKey("net_interest_income", Order = 25, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NetInterestIncome { get;  }

    [ColumnWithKey("extraordinary_items", Order = 26, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? ExtraordinaryItems { get;  }

    [ColumnWithKey("non_recurring", Order = 27, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NonRecurring { get;  }

    [ColumnWithKey("other_items", Order = 28, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? OtherItems { get;  }

    [ColumnWithKey("income_tax_expense", Order = 29, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? IncomeTaxExpense { get;  }

    [ColumnWithKey("total_revenue", Order = 30, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TotalRevenue { get;  }

    [ColumnWithKey("total_operating_expenses", Order = 31, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TotalOperatingExpenses { get;  }

    [ColumnWithKey("cost_of_revenue", Order = 32, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? CostOfRevenue { get;  }

    [ColumnWithKey("total_other_income_expense_net", Order = 33, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? TotalOtherIncomeExpenseNet { get;  }

    [ColumnWithKey("discontinued_operations", Order = 34, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? DiscontinuedOperations { get;  }

    [ColumnWithKey("net_income_from_continuing_ops", Order = 35, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NetIncomeFromContinuingOps { get;  }

    [ColumnWithKey("net_income_applicable_to_common_shares", Order = 36, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? NetIncomeApplicableToCommonShares { get;  }

    [ColumnWithKey("preferred_stock_and_other_adjustments", Order = 37, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? PreferredStockAndOtherAdjustments { get;  }

    [ColumnWithKey("utc_timestamp", Order = 38, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get;  }
}
