using Import.Infrastructure.Tests.Fixtures;

namespace Import.Infrastructure.PostgreSQL.Tests;

[Collection("Integration Tests")]
public class PriceActionsTests : TestBase
{
    public PriceActionsTests(DbFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task SavePriceActions()
    {
        var sut = fixture.ImportDbContext;

        int num = 3;

        var priceActions = CreatePriceActions(num).ToArray();

        var beforeCount = await sut.CountPriceActionsAsync();
        await sut.SavePriceActionsAsync("TEST", "NYSE", priceActions);
        var afterCount = await sut.CountPriceActionsAsync();

        Assert.Equal(beforeCount + num, afterCount);
    }

    private static IEnumerable<EodHistoricalData.Sdk.Models.PriceAction> CreatePriceActions(int numberToCreate = 1)
    {
        DateOnly date = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1 * Random.Next(2, 100)));

        for (int i = 0; i < numberToCreate; i++)
        {
            date = date.AddDays(1);
            yield return new EodHistoricalData.Sdk.Models.PriceAction()
            {

                AdjustedClose = 1.0M,
                Open = 1.0M,
                High = 1.0M,
                Low = 1.0M,
                Close = 1.0M,
                Date = date,
                Volume = 100_000L,
            };
        }
    }
}
