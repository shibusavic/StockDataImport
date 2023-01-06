using Import.Infrastructure.Tests.Fixtures;
using System.Text.Json;

namespace Import.Infrastructure.Tests.PostgreSQL;

[Collection("Integration Tests")]
public class OptionsTests : TestBase
{
    public OptionsTests(DbFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task SaveOptions()
    {
        string json = File.ReadAllText(@"MockData/aapl-options.json");

        var options = JsonSerializer.Deserialize<EodHistoricalData.Sdk.Models.Options.OptionsCollection>(json,
            serializerOptions);

        var sut = fixture.ImportDbContext;

        await sut.ClearOptionsAsync();

        var optionsCount = await sut.CountOptionsAsync();
        var optionsDataCount = await sut.CountOptionDataAsync();
        var optionsContractCount = await sut.CountOptionContractAsync();

        Assert.Equal(0, optionsCount);
        Assert.Equal(0, optionsDataCount);
        Assert.Equal(0, optionsContractCount);

        await sut.SaveOptionsAsync(options);

        optionsCount = await sut.CountOptionsAsync();
        optionsDataCount = await sut.CountOptionDataAsync();
        optionsContractCount = await sut.CountOptionContractAsync();

        Assert.True(optionsCount > 0);
        Assert.True(optionsDataCount> 0);
        Assert.True(optionsContractCount> 0);
    }

    private EodHistoricalData.Sdk.Models.Options.OptionsCollection CreateOptions()
    {

        string json = File.ReadAllText(@"MockData/aapl-options.json");
        Assert.NotNull(json);

        return JsonSerializer.Deserialize<EodHistoricalData.Sdk.Models.Options.OptionsCollection>(json, serializerOptions);
    }
}