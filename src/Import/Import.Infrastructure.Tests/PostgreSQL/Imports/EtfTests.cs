using Import.Infrastructure.IntegrationTests.Fixtures;
using System.Text.Json;

namespace Import.Infrastructure.IntegrationTests.PostgreSQL.Imports;

[Collection("Integration Tests")]
public class EtfTests : TestBase
{
    public EtfTests(DbFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Save_Etf()
    {
        string json = File.ReadAllText(@"MockData/spy-fundamentals.json");

        var etf = JsonSerializer.Deserialize<EodHistoricalData.Sdk.Models.Fundamentals.Etf.EtfFundamentalsCollection>(json,
            serializerOptions);

        var sut = fixture.ImportDbContext;

        await sut.DeleteEtfAsync("SPY", "NYSE ARCA", "ETF", "SPDR® S&P 500");

        var etfId = await sut.GetEtfIdAsync("SPY", "NYSE ARCA", "ETF", "SPDR® S&P 500");

        List<int> counts = new()
        {
            await sut.CountEtfsAsync("global_id", etfId),
            await sut.CountEtfTechnicalsAsync("etf_id", etfId),
            await sut.CountEtfMarketCapitalizationAsync("etf_id", etfId),
            await sut.CountEtfAssetAllocationAsync("etf_id", etfId),
            await sut.CountEtfWorldRegionsAsync("etf_id", etfId),
            await sut.CountEtfSectorWeightsAsync("etf_id", etfId),
            await sut.CountEtfFixedIncomesAsync("etf_id", etfId),
            await sut.CountEtfTopTenHoldingsAsync("etf_id", etfId),
            await sut.CountEtfHoldingsAsync("etf_id", etfId),
            await sut.CountEtfValuationGrowthAsync("etf_id", etfId),
            await sut.CountEtfMorningStarAsync("etf_id", etfId),
            await sut.CountEtfPerformanceAsync("etf_id", etfId)
        };

        foreach (var c in counts)
        {
            Assert.Equal(0, c);
        }

        await sut.SaveEtfAsync(etf);

        etfId = await sut.GetEtfIdAsync("SPY", "NYSE ARCA", "ETF", "SPDR® S&P 500");

        counts = new()
        {
            await sut.CountEtfsAsync("global_id", etfId),
            await sut.CountEtfTechnicalsAsync("etf_id", etfId),
            await sut.CountEtfMarketCapitalizationAsync("etf_id", etfId),
            await sut.CountEtfAssetAllocationAsync("etf_id", etfId),
            await sut.CountEtfWorldRegionsAsync("etf_id", etfId),
            await sut.CountEtfSectorWeightsAsync("etf_id", etfId),
            await sut.CountEtfFixedIncomesAsync("etf_id", etfId),
            await sut.CountEtfTopTenHoldingsAsync("etf_id", etfId),
            await sut.CountEtfHoldingsAsync("etf_id", etfId),
            await sut.CountEtfValuationGrowthAsync("etf_id", etfId),
            await sut.CountEtfMorningStarAsync("etf_id", etfId),
            await sut.CountEtfPerformanceAsync("etf_id", etfId)
        };

        int index = 0;

        foreach (var c in counts)
        {
            Assert.True(c > 0, index.ToString());
            index++;
        }
    }
}
