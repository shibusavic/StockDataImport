using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "company_highlights", Schema = "public")]
internal class CompanyHighlights
{
    public CompanyHighlights(Guid companyId,
        EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.Highlights highlights)
    {
        CompanyId = companyId;
        MarketCapitalization = highlights.MarketCapitalization.GetValueOrDefault();
        MarketCapitalizationMln = highlights.MarketCapitalizationMln.GetValueOrDefault();
        Ebitda = highlights.Ebitda.GetValueOrDefault();
        PeRatio = highlights.PeRatio.GetValueOrDefault();
        PegRatio = highlights.PegRatio.GetValueOrDefault();
        WallStreetTargetPrice = highlights.WallStreetTargetPrice.GetValueOrDefault();
        BookValue = highlights.BookValue.GetValueOrDefault();
        DividendShare = highlights.DividendShare.GetValueOrDefault();
        DividendYield = highlights.DividendYield.GetValueOrDefault();
        EarningsShare = highlights.EarningsShare.GetValueOrDefault();
        EpsEstimateCurrentYear = highlights.EpsEstimateCurrentYear.GetValueOrDefault();
        EpsEstimateNextYear = highlights.EpsEstimateNextYear.GetValueOrDefault();
        EpsEstimateNextQuarter = highlights.EpsEstimateNextQuarter.GetValueOrDefault();
        EpsEstimateCurrentQuarter = highlights.EpsEstimateCurrentQuarter.GetValueOrDefault();
        MostRecentQuarter = highlights.MostRecentQuarter.GetValueOrDefault().ToDateTime(TimeOnly.MinValue);
        ProfitMargin = highlights.ProfitMargin.GetValueOrDefault();
        OperatingMarginTtm = highlights.OperatingMarginTtm.GetValueOrDefault();
        ReturnOnAssetsTtm = highlights.ReturnOnAssetsTtm.GetValueOrDefault();
        ReturnOnEquityTtm = highlights.ReturnOnEquityTtm.GetValueOrDefault();
        RevenueTtm = highlights.RevenueTtm.GetValueOrDefault();
        RevenuePerShareTtm = highlights.RevenuePerShareTtm.GetValueOrDefault();
        QuarterlyRevenueGrowthYoy = highlights.QuarterlyRevenueGrowthYoy.GetValueOrDefault();
        GrossProfitTtm = highlights.GrossProfitTtm.GetValueOrDefault();
        DilutedEpsTtm = highlights.DilutedEpsTtm.GetValueOrDefault();
        QuarterlyEarningsGrowthYoy = highlights.QuarterlyEarningsGrowthYoy.GetValueOrDefault();
        UtcTimestamp = DateTime.UtcNow;
    }

    public CompanyHighlights(
        Guid companyId,
        decimal? marketCapitalization,
        decimal? marketCapitalizationMln,
        decimal? ebitda,
        double? peRatio,
        double? pegRatio,
        decimal? wallStreetTargetPrice,
        double? bookValue,
        double? dividendShare,
        double? dividendYield,
        decimal? earningsShare,
        decimal? epsEstimateCurrentYear,
        decimal? epsEstimateNextYear,
        decimal? epsEstimateNextQuarter,
        decimal? epsEstimateCurrentQuarter,
        DateTime? mostRecentQuarter,
        double? profitMargin,
        double? operatingMarginTtm,
        double? returnOnAssetsTtm,
        double? returnOnEquityTtm,
        decimal? revenueTtm,
        decimal? revenuePerShareTtm,
        double? quarterlyRevenueGrowthYoy,
        decimal? grossProfitTtm,
        decimal? dilutedEpsTtm,
        double? quarterlyEarningsGrowthYoy,
        DateTime? utcTimestamp = null)
    {
        CompanyId = companyId;
        MarketCapitalization = marketCapitalization;
        MarketCapitalizationMln = marketCapitalizationMln;
        Ebitda = ebitda;
        PeRatio = peRatio;
        PegRatio = pegRatio;
        WallStreetTargetPrice = wallStreetTargetPrice;
        BookValue = bookValue;
        DividendShare = dividendShare;
        DividendYield = dividendYield;
        EarningsShare = earningsShare;
        EpsEstimateCurrentYear = epsEstimateCurrentYear;
        EpsEstimateNextYear = epsEstimateNextYear;
        EpsEstimateNextQuarter = epsEstimateNextQuarter;
        EpsEstimateCurrentQuarter = epsEstimateCurrentQuarter;
        MostRecentQuarter = mostRecentQuarter;
        ProfitMargin = profitMargin;
        OperatingMarginTtm = operatingMarginTtm;
        ReturnOnAssetsTtm = returnOnAssetsTtm;
        ReturnOnEquityTtm = returnOnEquityTtm;
        RevenueTtm = revenueTtm;
        RevenuePerShareTtm = revenuePerShareTtm;
        QuarterlyRevenueGrowthYoy = quarterlyRevenueGrowthYoy;
        GrossProfitTtm = grossProfitTtm;
        DilutedEpsTtm = dilutedEpsTtm;
        QuarterlyEarningsGrowthYoy = quarterlyEarningsGrowthYoy;
        UtcTimestamp = utcTimestamp ?? DateTime.UtcNow;
    }


    [ColumnWithKey("company_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid CompanyId { get; }

    [ColumnWithKey("market_capitalization", Order = 2, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? MarketCapitalization { get; }

    [ColumnWithKey("market_capitalization_mln", Order = 3, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? MarketCapitalizationMln { get; }

    [ColumnWithKey("ebitda", Order = 4, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? Ebitda { get; }

    [ColumnWithKey("pe_ratio", Order = 5, TypeName = "double precision", IsPartOfKey = false)]
    public double? PeRatio { get; }

    [ColumnWithKey("peg_ratio", Order = 6, TypeName = "double precision", IsPartOfKey = false)]
    public double? PegRatio { get; }

    [ColumnWithKey("wall_street_target_price", Order = 7, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? WallStreetTargetPrice { get; }

    [ColumnWithKey("book_value", Order = 8, TypeName = "double precision", IsPartOfKey = false)]
    public double? BookValue { get; }

    [ColumnWithKey("dividend_share", Order = 9, TypeName = "double precision", IsPartOfKey = false)]
    public double? DividendShare { get; }

    [ColumnWithKey("dividend_yield", Order = 10, TypeName = "double precision", IsPartOfKey = false)]
    public double? DividendYield { get; }

    [ColumnWithKey("earnings_share", Order = 11, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EarningsShare { get; }

    [ColumnWithKey("eps_estimate_current_year", Order = 12, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsEstimateCurrentYear { get; }

    [ColumnWithKey("eps_estimate_next_year", Order = 13, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsEstimateNextYear { get; }

    [ColumnWithKey("eps_estimate_next_quarter", Order = 14, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsEstimateNextQuarter { get; }

    [ColumnWithKey("eps_estimate_current_quarter", Order = 15, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? EpsEstimateCurrentQuarter { get; }

    [ColumnWithKey("most_recent_quarter", Order = 16, TypeName = "date", IsPartOfKey = false)]
    public DateTime? MostRecentQuarter { get; }

    [ColumnWithKey("profit_margin", Order = 17, TypeName = "double precision", IsPartOfKey = false)]
    public double? ProfitMargin { get; }

    [ColumnWithKey("operating_margin_ttm", Order = 18, TypeName = "double precision", IsPartOfKey = false)]
    public double? OperatingMarginTtm { get; }

    [ColumnWithKey("return_on_assets_ttm", Order = 19, TypeName = "double precision", IsPartOfKey = false)]
    public double? ReturnOnAssetsTtm { get; }

    [ColumnWithKey("return_on_equity_ttm", Order = 20, TypeName = "double precision", IsPartOfKey = false)]
    public double? ReturnOnEquityTtm { get; }

    [ColumnWithKey("revenue_ttm", Order = 21, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? RevenueTtm { get; }

    [ColumnWithKey("revenue_per_share_ttm", Order = 22, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? RevenuePerShareTtm { get; }

    [ColumnWithKey("quarterly_revenue_growth_yoy", Order = 23, TypeName = "double precision", IsPartOfKey = false)]
    public double? QuarterlyRevenueGrowthYoy { get; }

    [ColumnWithKey("gross_profit_ttm", Order = 24, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? GrossProfitTtm { get; }

    [ColumnWithKey("diluted_eps_ttm", Order = 25, TypeName = "numeric", IsPartOfKey = false)]
    public decimal? DilutedEpsTtm { get; }

    [ColumnWithKey("quarterly_earnings_growth_yoy", Order = 26, TypeName = "double precision", IsPartOfKey = false)]
    public double? QuarterlyEarningsGrowthYoy { get; }

    [ColumnWithKey("utc_timestamp", Order = 27, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }
}
