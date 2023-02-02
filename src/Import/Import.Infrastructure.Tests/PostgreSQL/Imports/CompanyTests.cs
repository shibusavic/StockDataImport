using Import.Infrastructure.Tests.Fixtures;
using System.Text.Json;

namespace Import.Infrastructure.PostgreSQL.Tests;

[Collection("Integration Tests")]
public class CompanyTests : TestBase
{
    public CompanyTests(DbFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Save_Gdiv()
    {
        string json = File.ReadAllText(@"MockData/gdiv-fundamentals.json");

        var gdiv = JsonSerializer.Deserialize<EodHistoricalData.Sdk.Models.Fundamentals.Etf.EtfFundamentalsCollection>(json,
            serializerOptions);

        var sut = fixture.ImportDbContext;

        await sut.DeleteCompanyAsync("GDIV", "NYSE", "ETF", "Harbor Dividend Growth Leaders ETF");

        var id = await sut.GetCompanyIdAsync("GDIV", "NYSE", "ETF", "Harbor Dividend Growth Leaders ETF");

        List<int> counts = new()
        {
            await sut.CountEtfsAsync("global_id", id),
            await sut.CountEtfTechnicalsAsync("etf_id", id),
            await sut.CountEtfMarketCapitalizationAsync("etf_id", id),
            await sut.CountEtfAssetAllocationAsync("etf_id", id),
            await sut.CountEtfWorldRegionsAsync("etf_id", id),
            await sut.CountEtfSectorWeightsAsync("etf_id", id),
            await sut.CountEtfFixedIncomesAsync("etf_id", id),
            await sut.CountEtfTopTenHoldingsAsync("etf_id", id),
            await sut.CountEtfHoldingsAsync("etf_id", id),
            await sut.CountEtfValuationGrowthAsync("etf_id", id),
            await sut.CountEtfMorningStarAsync("etf_id", id),
            await sut.CountEtfPerformanceAsync("etf_id", id)
        };

        foreach (var c in counts)
        {
            Assert.Equal(0, c);
        }

        await sut.SaveEtfAsync(gdiv);

        id = await sut.GetEtfIdAsync("GDIV", "NYSE", "ETF", "Harbor Dividend Growth Leaders ETF");

        counts = new()
        {
            await sut.CountEtfsAsync("global_id", id),
            await sut.CountEtfTechnicalsAsync("etf_id", id),
            await sut.CountEtfMarketCapitalizationAsync("etf_id", id),
            await sut.CountEtfAssetAllocationAsync("etf_id", id),
            await sut.CountEtfWorldRegionsAsync("etf_id", id),
            await sut.CountEtfSectorWeightsAsync("etf_id", id),
            await sut.CountEtfFixedIncomesAsync("etf_id", id),
            await sut.CountEtfTopTenHoldingsAsync("etf_id", id),
            await sut.CountEtfHoldingsAsync("etf_id", id),
            await sut.CountEtfValuationGrowthAsync("etf_id", id),
            await sut.CountEtfMorningStarAsync("etf_id", id),
            await sut.CountEtfPerformanceAsync("etf_id", id)
        };

        int index = 0;

        foreach (var c in counts)
        {
            Assert.True(c > 0, index.ToString());
            index++;
        }
    }

    [Fact]
    public async Task Save_Company()
    {
        string json = File.ReadAllText(@"MockData/aapl-fundamentals.json");

        var company = JsonSerializer.Deserialize<EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.FundamentalsCollection>(json,
            serializerOptions);

        var sut = fixture.ImportDbContext;

        await sut.DeleteCompanyAsync("AAPL", "NASDAQ", "Common Stock", "Apple Inc");

        var companyId = await sut.GetCompanyIdAsync("AAPL", "NASDAQ", "Common Stock", "Apple Inc");

        List<int> counts = new()
        {
            await sut.CountCompaniesAsync("global_id", companyId),
            await sut.CountCompanyAddressesAsync("company_id", companyId),
            await sut.CountCompanyListingsAsync("company_id", companyId),
            await sut.CountCompanyOfficersAsync("company_id", companyId),
            await sut.CountCompanyHighlightsAsync("company_id", companyId),
            await sut.CountCompanyValuationsAsync("company_id", companyId),
            await sut.CountCompanyShareStatsAsync("company_id", companyId),
            await sut.CountCompanyTechnicalsAsync("company_id", companyId),
            await sut.CountCompanyDividendsAsync("company_id", companyId),
            await sut.CountCompanyDividendsByYearAsync("company_id", companyId),
            await sut.CountCompanyAnalystRatingsAsync("company_id", companyId),
            await sut.CountCompanyHoldersAsync("company_id", companyId),
            await sut.CountCompanyInsiderTransactionsAsync("company_id", companyId),
            await sut.CountCompanyEsgScoresAsync("company_id", companyId),
            await sut.CountCompanyEsgActivitiesAsync("company_id", companyId),
            await sut.CountCompanyOutstandingSharesAsync("company_id", companyId),
            await sut.CountCompanyEarningsHistoryAsync("company_id", companyId),
            await sut.CountCompanyEarningsTrendsAsync("company_id", companyId),
            await sut.CountCompanyEarningsAnnualAsync("company_id", companyId),
            await sut.CountCompanyBalanceSheetsAsync("company_id", companyId),
            await sut.CountCompanyCashFlowsAsync("company_id", companyId),
            await sut.CountCompanyIncomeStatementsAsync("company_id", companyId)
        };

        foreach (var c in counts)
        {
            Assert.Equal(0, c);
        }

        await sut.SaveCompanyAsync(company);

        companyId = await sut.GetCompanyIdAsync("AAPL", "NASDAQ", "Common Stock", "Apple Inc");

        counts = new()
        {
            await sut.CountCompaniesAsync("global_id", companyId),
            await sut.CountCompanyAddressesAsync("company_id", companyId),
            await sut.CountCompanyListingsAsync("company_id", companyId),
            await sut.CountCompanyOfficersAsync("company_id", companyId),
            await sut.CountCompanyHighlightsAsync("company_id", companyId),
            await sut.CountCompanyValuationsAsync("company_id", companyId),
            await sut.CountCompanyShareStatsAsync("company_id", companyId),
            await sut.CountCompanyTechnicalsAsync("company_id", companyId),
            await sut.CountCompanyDividendsAsync("company_id", companyId),
            await sut.CountCompanyDividendsByYearAsync("company_id", companyId),
            await sut.CountCompanyAnalystRatingsAsync("company_id", companyId),
            await sut.CountCompanyHoldersAsync("company_id", companyId),
            await sut.CountCompanyInsiderTransactionsAsync("company_id", companyId),
            await sut.CountCompanyEsgScoresAsync("company_id", companyId),
            await sut.CountCompanyEsgActivitiesAsync("company_id", companyId),
            await sut.CountCompanyOutstandingSharesAsync("company_id", companyId),
            await sut.CountCompanyEarningsHistoryAsync("company_id", companyId),
            await sut.CountCompanyEarningsTrendsAsync("company_id", companyId),
            await sut.CountCompanyEarningsAnnualAsync("company_id", companyId),
            await sut.CountCompanyBalanceSheetsAsync("company_id", companyId),
            await sut.CountCompanyCashFlowsAsync("company_id", companyId),
            await sut.CountCompanyIncomeStatementsAsync("company_id", companyId)
        };

        int index = 0;

        foreach (var c in counts)
        {
            Assert.True(c > 0, index.ToString());
            index++;
        }
    }
}
