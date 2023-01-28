using EodHistoricalData.Sdk.Events;
using EodHistoricalData.Sdk.Models.Bulk;

namespace EodHistoricalData.Sdk.Tests;

[Collection("API Tests")]
public class BulkDataTests : BaseTest
{
    [Fact]
    public async Task GetBulkHistoricalDataForExchangeAsync_BadApiKey_ThrowsUnauthorizedAccessException()
    {
        List<ApiResponseException> excs = new();

        ApiEventPublisher.RaiseApiResponseEventHandler += (sender, e) =>
        {
            if (e.ApiResponseException != null)
            {
                excs.Add(e.ApiResponseException);
            }
        };

        var dataClient = new DataClient(Guid.NewGuid().ToString()[..5]);

        Assert.Empty(await dataClient.GetBulkHistoricalDataForExchangeAsync("NYSE"));
        Assert.True(excs.Count > 0);
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetBulkHistoricalDataForExchangeAsync_BadExchange_Throws()
    {
        var dataClient = new DataClient(apiKey);

        Assert.Empty(await dataClient.GetBulkHistoricalDataForExchangeAsync(Guid.NewGuid().ToString()[..4]));
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetBulkHistoricalDataForExchangeAsync_LastDay()
    {
        DateOnly dt = DateOnly.FromDateTime(DateTime.Now);
        while (dt.DayOfWeek is not DayOfWeek.Tuesday or DayOfWeek.Wednesday)
        {
            dt = dt.AddDays(-1);
        }

        var dataClient = new DataClient(apiKey);

        var bulkLastDay = await dataClient.GetBulkHistoricalDataForExchangeAsync("NYSE", date: dt);

        Assert.NotNull(bulkLastDay);
        Assert.NotEmpty(bulkLastDay);

        if (bulkLastDay != null)
        {
            // if you run this test in the early part of a day in the middle of a week, it might fail because the "last day" is yesterday.
            // if you run it at the end of the day, "last day" might be today.
            Assert.True(dt >= bulkLastDay.First().Date);
            TestBulkPriceActionCollection(bulkLastDay);
        }
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetBulkHistoricalDataForExchangeAsync_SpecificDay()
    {
        DateOnly dt = DateOnly.FromDateTime(DateTime.Now);
        while (dt.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday or DayOfWeek.Sunday or DayOfWeek.Monday)
        {
            dt = dt.AddDays(-1);
        }

        dt = dt.AddDays(-7);

        var dataClient = new DataClient(apiKey);

        var bulkSpecificDay = await dataClient.GetBulkHistoricalDataForExchangeAsync("NYSE", date: new DateOnly(dt.Year, dt.Month, dt.Day));

        Assert.NotNull(bulkSpecificDay);
        Assert.NotEmpty(bulkSpecificDay);

        if (bulkSpecificDay != null)
        {
            Assert.Equal(dt, bulkSpecificDay.First().Date);
            TestBulkPriceActionCollection(bulkSpecificDay);
        }
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetBulkHistoricalDataForExchangeAsync_SpecificSymbols()
    {
        List<string> symbols = new() { "ACEL", "ACI", "ACB" };
        var dataClient = new DataClient(apiKey);

        var bulkSpecificDay = await dataClient.GetBulkHistoricalDataForExchangeAsync("NYSE", symbols: symbols);

        Assert.NotNull(bulkSpecificDay);
        Assert.NotEmpty(bulkSpecificDay);

        if (bulkSpecificDay != null)
        {
            foreach (var priceAction in bulkSpecificDay)
            {
                Assert.Contains(priceAction.Code, symbols);
            }
        }
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetBulkHistoricalDataForExchangeStringAsync_StringSerializable()
    {
        var dataClient = new DataClient(apiKey);

        var bulkLastDayString = await dataClient.GetBulkHistoricalDataForExchangeStringAsync("NYSE");

        Assert.NotNull(bulkLastDayString);

        var bulk = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<BulkPriceAction>>(bulkLastDayString, serializerOptions);

        Assert.NotNull(bulk);
        Assert.NotEmpty(bulk);
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetExtendedBulkHistoricalDataForExchangeStringAsync_StringSerializable()
    {
        var dataClient = new DataClient(apiKey);

        var extendedBulkLastDayString = await dataClient.GetExtendedBulkHistoricalDataForExchangeStringAsync("NYSE");

        Assert.NotNull(extendedBulkLastDayString);

        var bulk = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<BulkExtendedPriceAction>>(extendedBulkLastDayString, serializerOptions);

        Assert.NotNull(bulk);
        Assert.NotEmpty(bulk);
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetExtendedBulkHistoricalDataForExchangeAsync_LastDay()
    {
        DateOnly dt = DateOnly.FromDateTime(DateTime.Now);
        while (dt.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday or DayOfWeek.Sunday or DayOfWeek.Monday)
        {
            dt = dt.AddDays(-1);
        }

        var dataClient = new DataClient(apiKey);

        var bulkLastDay = await dataClient.GetExtendedBulkHistoricalDataForExchangeAsync("NYSE");

        Assert.NotNull(bulkLastDay);
        Assert.NotEmpty(bulkLastDay);
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetExtendedBulkHistoricalDataForExchangeAsync_SpecificDay()
    {
        DateOnly dt = DateOnly.FromDateTime(DateTime.Now);
        while (dt.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday or DayOfWeek.Sunday or DayOfWeek.Monday)
        {
            dt = dt.AddDays(-1);
        }

        dt = dt.AddDays(-7);

        var dataClient = new DataClient(apiKey);

        var bulkSpecificDay = await dataClient.GetExtendedBulkHistoricalDataForExchangeAsync("NYSE", date: new DateOnly(dt.Year, dt.Month, dt.Day));

        Assert.NotNull(bulkSpecificDay);
        Assert.NotEmpty(bulkSpecificDay);
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetExtendedBulkHistoricalDataForExchangeAsync_SpecificSymbols()
    {
        List<string> symbols = new() { "ACEL", "ACI", "ACB" };
        var dataClient = new DataClient(apiKey);

        var bulkSpecificDay = await dataClient.GetExtendedBulkHistoricalDataForExchangeAsync("NYSE", symbols: symbols);

        Assert.NotNull(bulkSpecificDay);
        Assert.NotEmpty(bulkSpecificDay);

        if (bulkSpecificDay != null)
        {
            foreach (var priceAction in bulkSpecificDay)
            {
                Assert.Contains(priceAction.Code, symbols);
            }
        }
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetBulkDividendsForExchangeStringAsync_StringSerializable()
    {
        var dataClient = new DataClient(apiKey);

        var bulkDividendsString = await dataClient.GetBulkDividendsForExchangeStringAsync("NASDAQ", date: new(2021, 12, 31));

        Assert.NotNull(bulkDividendsString);

        var bulk = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<BulkDividend>>(bulkDividendsString, serializerOptions);

        Assert.NotNull(bulk);
        Assert.NotEmpty(bulk);
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetBulkSplitsForExchangeStringAsync_StringSerializable()
    {
        var dataClient = new DataClient(apiKey);

        var bulkSplitsString = await dataClient.GetBulkSplitsForExchangeStringAsync("NYSE", date: new(2021, 11, 18));

        Assert.NotNull(bulkSplitsString);

        var bulk = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<BulkSplit>>(bulkSplitsString, serializerOptions);

        Assert.NotNull(bulk);
        Assert.NotEmpty(bulk);
    }

    private static void TestBulkPriceActionCollection(IEnumerable<BulkPriceAction> priceActions)
    {
        Assert.NotEmpty(priceActions);

        foreach (var priceAction in priceActions)
        {
            Assert.NotNull(priceAction.Code);
            Assert.NotNull(priceAction.ExchangeShortName);
            Assert.NotEqual(default, priceAction.Date);
        }
    }
}