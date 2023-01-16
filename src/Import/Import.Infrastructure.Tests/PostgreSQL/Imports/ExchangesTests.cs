using Import.Infrastructure.Tests.Fixtures;

namespace Import.Infrastructure.PostgreSQL.Tests;

[Collection("Integration Tests")]
public class ExchangesTests : TestBase
{
    public ExchangesTests(DbFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task SaveExchanges()
    {
        var sut = fixture.ImportDbContext;

        int num = 3;

        var exchanges = CreateExchanges(num).ToArray();

        var beforeCount = await sut.CountExchangesAsync();
        await sut.SaveExchangesAsync(exchanges);
        var afterCount = await sut.CountExchangesAsync();

        Assert.Equal(beforeCount + num, afterCount);
    }

    protected static IEnumerable<EodHistoricalData.Sdk.Models.Exchange> CreateExchanges(int numberToCreate = 1)
    {
        for (int i = 0; i < numberToCreate; i++)
        {
            var code = Guid.NewGuid().ToString()[..4];
            yield return new EodHistoricalData.Sdk.Models.Exchange()
            {
                Code = code,
                Country = "USA",
                Currency = "USD",
                Name = "TEST",
                OperatingMic = "123",
                CountryIso2 = "321",
                CountryIso3 = "3242"
            };
        }
    }
}
