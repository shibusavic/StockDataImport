using Import.Infrastructure.Tests.Fixtures;
using System.Text.Json;

namespace Import.Infrastructure.Tests.PostgreSQL;

[Collection("Integration Tests")]
public class CalendarTests : TestBase
{
    public CalendarTests(DbFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Save_Ipos()
    {
        string json = File.ReadAllText(@"MockData/ipos.json");

        var ipos = JsonSerializer.Deserialize<EodHistoricalData.Sdk.Models.Calendar.IpoCollection>(json, serializerOptions);

        var sut = fixture.ImportDbContext;

        await sut.DeleteAsync("public.calendar_ipos");

        var count = await sut.CountIposAsync();

        Assert.Equal(0, count);

        await sut.SaveIpos(ipos);

        count = await sut.CountIposAsync();

        Assert.True(count > 0);
    }

    [Fact]
    public async Task Save_Earnings()
    {
        string json = File.ReadAllText(@"MockData/earnings.json");

        var earnings = JsonSerializer.Deserialize<EodHistoricalData.Sdk.Models.Calendar.EarningsCollection>(json, serializerOptions);

        var sut = fixture.ImportDbContext;

        await sut.DeleteAsync("public.calendar_earnings");

        var count = await sut.CountEarningsAsync();

        Assert.Equal(0, count);

        await sut.SaveEarnings(earnings);

        count = await sut.CountEarningsAsync();

        Assert.True(count > 0);
    }

    [Fact]
    public async Task Save_Trends()
    {
        string json = File.ReadAllText(@"MockData/trends.json");

        var dao = JsonSerializer.Deserialize<EodHistoricalData.Sdk.DataClient.TrendCollectionDao>(json, serializerOptions);

        var sut = fixture.ImportDbContext;

        await sut.DeleteAsync("public.calendar_trends");

        var count = await sut.CountTrendsAsync();

        Assert.Equal(0, count);

        var trends = new EodHistoricalData.Sdk.Models.Calendar.TrendCollection()
        {
            Type = dao.Type,
            Description = dao.Description,
            Symbols = dao.Symbols,
            Trends = dao.Trends.First()
        };

        await sut.SaveTrends(trends);

        count = await sut.CountTrendsAsync();

        Assert.True(count > 0);
    }
}
