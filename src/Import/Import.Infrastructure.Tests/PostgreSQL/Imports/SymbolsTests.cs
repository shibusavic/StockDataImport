using EodHistoricalData.Sdk.Models;
using Import.Infrastructure.IntegrationTests.Fixtures;

namespace Import.Infrastructure.IntegrationTests.PostgreSQL.Imports;

[Collection("Integration Tests")]
public class SymbolsTests : IClassFixture<DbFixture>
{
    private readonly DbFixture fixture;

    public SymbolsTests(DbFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task SaveSymbols()
    {
        var sut = fixture.ImportDbContext;

        int num = 3;

        var symbols = CreateSymbols(num).ToArray();

        var beforeCount = await sut.CountSymbolsAsync();
        await sut.SaveSymbolsAsync(symbols);
        var afterCount = await sut.CountSymbolsAsync();

        Assert.Equal(beforeCount + num, afterCount);
    }

    private IEnumerable<Symbol> CreateSymbols(int numberToCreate = 1)
    {
        for (int i = 0; i < numberToCreate; i++)
        {
            string name = Guid.NewGuid().ToString().ToUpper().Replace("-", "");

            yield return new Symbol()
            {
                Code = name[..4],
                Country = "USA",
                Currency = "USD",
                Exchange = "Test",
                Name = name,
                Type = "Common Stock"
            };
        }
    }
}
