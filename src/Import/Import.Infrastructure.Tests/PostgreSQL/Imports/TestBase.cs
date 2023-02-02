using EodHistoricalData.Sdk;
using Import.Infrastructure.Tests.Fixtures;
using System.Text.Json;

namespace Import.Infrastructure.PostgreSQL.Tests;

public abstract class TestBase : IClassFixture<DbFixture>
{
    protected readonly DbFixture fixture;
    protected readonly JsonSerializerOptions serializerOptions;
    protected readonly static Random Random = new(Guid.NewGuid().GetHashCode());

    public TestBase(DbFixture fixture)
    {
        this.fixture = fixture;

        serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };

        serializerOptions.Converters.Add(new NullableStringJsonConverter());

        serializerOptions.Converters.Add(new DateOnlyJsonConverter());
        serializerOptions.Converters.Add(new NullableDateOnlyJsonConverter());

        serializerOptions.Converters.Add(new DateTimeJsonConverter());
        serializerOptions.Converters.Add(new NullableDateTimeJsonConverter());

        serializerOptions.Converters.Add(new DoubleJsonConverter());
        serializerOptions.Converters.Add(new NullableDoubleJsonConverter());

        serializerOptions.Converters.Add(new DecimalJsonConverter());
        serializerOptions.Converters.Add(new NullableDecimalJsonConverter());

        serializerOptions.Converters.Add(new BooleanJsonConverter());
        serializerOptions.Converters.Add(new NullableBooleanJsonConverter());

        serializerOptions.Converters.Add(new LongJsonConverter());
        serializerOptions.Converters.Add(new NullableLongJsonConverter());
    }

    internal async Task LoadImportsNyseExchangesAndSymbolsAsync(ImportsDbContext dbContext)
    {
        string json = File.ReadAllText(@"MockData/exchanges.json");
        var exchanges = JsonSerializer.Deserialize<EodHistoricalData.Sdk.Models.Exchange[]>(json, serializerOptions);

        Assert.NotNull(exchanges);
        Assert.NotEmpty(exchanges);

        await dbContext.SaveExchangesAsync(exchanges);

        json = File.ReadAllText(@"MockData/nyse-symbols.json");

        var symbols = JsonSerializer.Deserialize<EodHistoricalData.Sdk.Models.Symbol[]>(json, serializerOptions);

        Assert.NotNull(symbols);
        Assert.NotEmpty(symbols);

        await dbContext.SaveSymbolsAsync(symbols, "US");
    }

    internal async Task LoadImportsNasdaqExchangesAndSymbolsAsync(ImportsDbContext dbContext)
    {
        string json = File.ReadAllText(@"MockData/exchanges.json");
        var exchanges = JsonSerializer.Deserialize<EodHistoricalData.Sdk.Models.Exchange[]>(json, serializerOptions);

        Assert.NotNull(exchanges);
        Assert.NotEmpty(exchanges);

        await dbContext.SaveExchangesAsync(exchanges);

        json = File.ReadAllText(@"MockData/nasdaq-symbols.json");

        var symbols = JsonSerializer.Deserialize<EodHistoricalData.Sdk.Models.Symbol[]>(json, serializerOptions);

        Assert.NotNull(symbols);
        Assert.NotEmpty(symbols);

        await dbContext.SaveSymbolsAsync(symbols, "US");
    }

    internal async Task LoadImportsAaplCompanyAsync(ImportsDbContext dbContext)
    {
        string json = File.ReadAllText(@"MockData/aapl-fundamentals.json");
        var companies = JsonSerializer.Deserialize<EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.FundamentalsCollection>(json, serializerOptions);

        Assert.NotNull(companies.General.Name);

        await dbContext.SaveCompanyAsync(companies);
    }
}
