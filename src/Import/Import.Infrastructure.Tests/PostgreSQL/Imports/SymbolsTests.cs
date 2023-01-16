using EodHistoricalData.Sdk.Models;
using Import.Infrastructure.Tests.Fixtures;

namespace Import.Infrastructure.PostgreSQL.Tests;

[Collection("Integration Tests")]
public class SymbolsTests : TestBase
{
    public SymbolsTests(DbFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task SaveSymbols()
    {
        var sut = fixture.ImportDbContext;

        int num = 3;

        var symbols = CreateSymbols(num).ToArray();

        var beforeCount = await sut.CountSymbolsAsync();
        await sut.SaveSymbolsAsync(symbols, "US");
        var afterCount = await sut.CountSymbolsAsync();

        Assert.Equal(beforeCount + num, afterCount);
    }

    [Fact]
    public async Task MetaData_Hydrated()
    {
        var sut = fixture.ImportDbContext;

        await LoadImportsNyseExchangesAndSymbolsAsync(sut);

        var metaData = (await sut.GetSymbolMetaDataAsync()).ToArray();

        Assert.NotNull(metaData);
        Assert.NotEmpty(metaData);

        await sut.SavePriceActionsAsync(metaData[0].Symbol, metaData[0].Exchange ?? "None", new PriceAction[] {
            new PriceAction()
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                Close = 6M,
                Open = 9M,
                High = 11M,
                Low = 6M,
                AdjustedClose = 6M,
                Volume = 200_000
            },
            new PriceAction()
            {
                Date = DateOnly.FromDateTime(DateTime.Now),
                Close = 10M,
                Open = 9M,
                High = 11M,
                Low = 8M,
                AdjustedClose = 10M,
                Volume = 100_000
            }
        });

        metaData = (await sut.GetSymbolMetaDataAsync()).ToArray();

        Assert.Null(metaData[0].LastUpdatedCompany); // no company check in this test.
        Assert.Equal(10M, metaData[0].LastTrade.Close);
        Assert.NotNull(metaData[0].LastTrade.Start);
    }

    [Fact]
    public async Task MetaData_CompanyExists_LastUpdatesNotNull()
    {
        var sut = fixture.ImportDbContext;

        await LoadImportsNasdaqExchangesAndSymbolsAsync(sut);

        await LoadImportsAaplCompanyAsync(sut);

        var metaData = (await sut.GetSymbolMetaDataAsync()).ToArray();

        Assert.NotNull(metaData);
        Assert.NotEmpty(metaData);

        var item = metaData.FirstOrDefault(d => d.Symbol == "AAPL");

        Assert.NotNull(item);

        Assert.NotNull(item.LastUpdatedCompany);
        Assert.NotNull(item.LastUpdatedIncomeStatement);
    }

    [Fact]
    public async Task SaveAndGet_SymbolToIgnore()
    {
        var sut = fixture.ImportDbContext;

        var beforeCount = await sut.CountSymbolsToIgnoreAsync();

        await sut.SaveSymbolToIgnore(CreateSymbolToIgnore());

        var afterCount = await sut.CountSymbolsToIgnoreAsync();

        Assert.Equal(beforeCount + 1, afterCount);
    }

    [Fact]
    public async Task SaveAndGet_SymbolsToIgnore()
    {
        var sut = fixture.ImportDbContext;

        var beforeCount = await sut.CountSymbolsToIgnoreAsync();

        await sut.SaveSymbolsToIgnore(CreateSymbolsToIgnore(5));

        var afterCount = await sut.CountSymbolsToIgnoreAsync();

        Assert.Equal(beforeCount + 5, afterCount);
    }

    private static IgnoredSymbol CreateSymbolToIgnore()
    {
        return new IgnoredSymbol(Guid.NewGuid().ToString()[0..4], "TEST", "REASON");
    }

    private static IEnumerable<IgnoredSymbol> CreateSymbolsToIgnore(int number = 5)
    {
        for (int i = 0; i < number; i++)
        {
            yield return CreateSymbolToIgnore();
        }
    }

    private static IEnumerable<Symbol> CreateSymbols(int numberToCreate = 1)
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
