using Import.Infrastructure.IntegrationTests.Fixtures;

namespace Import.Infrastructure.IntegrationTests.PostgreSQL.Imports;

[Collection("Integration Tests")]
public class DividendsTests : TestBase
{
    public DividendsTests(DbFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task SaveDividends()
    {
        var sut = fixture.ImportDbContext;

        int num = 3;

        var dividends = CreateDividends(num).ToArray();

        var beforeCount = await sut.CountDividendsAsync();
        await sut.SaveDividendsAsync("TEST", "NYSE", dividends);
        var afterCount = await sut.CountDividendsAsync();

        Assert.Equal(beforeCount + num, afterCount);
    }

    private static IEnumerable<EodHistoricalData.Sdk.Models.Dividend> CreateDividends(int numberToCreate = 1)
    {
        DateOnly date = DateOnly.FromDateTime(DateTime.Now.AddMonths(-1 * Random.Next(2, 100)));

        for (int i = 0; i < numberToCreate; i++)
        {
            date = date.AddDays(Random.Next(1, 15));
            yield return new EodHistoricalData.Sdk.Models.Dividend()
            {
                Currency = "USD",
                Date = date,
                DeclarationDate = date.AddMonths(-1),
                PaymentDate = date.AddDays(15),
                Period = "Q1",
                RecordDate = date.AddMonths(-1),
                UnadjustedValue = 1.0M,
                Value = 1.0M
            };
        }
    }
}
