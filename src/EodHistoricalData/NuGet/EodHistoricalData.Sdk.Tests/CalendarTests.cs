using EodHistoricalData.Sdk.Events;
using EodHistoricalData.Sdk.Models.Calendar;

namespace EodHistoricalData.Sdk.Tests;

[Collection("API Tests")]
public class CalendarTests : BaseTest
{
    [Fact]
    public async Task GetEarningsAsync_BadApiKey_ThrowsUnauthorizedAccessException()
    {
        var dataClient = new DataClient(Guid.NewGuid().ToString()[..5]);

        List<ApiResponseException> excs = new();

        ApiEventPublisher.RaiseApiResponseEventHandler += (sender, e) =>
        {
            if (e.ApiResponseException != null)
            {
                excs.Add(e.ApiResponseException);
            }
        };

        Assert.Equal(new EarningsCollection(), await dataClient.GetEarningsAsync());
        Assert.Single(excs);
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetEarningsAsync_Valid_NotEmpty()
    {
        var dataClient = new DataClient(apiKey);
        var earnings = await dataClient.GetEarningsAsync();

        Assert.Equal("Earnings", earnings.Type);
        Assert.NotNull(earnings.Earnings);
        Assert.NotEmpty(earnings.Earnings);
    }

    [Fact]
    public async Task GetTrendsForSymbolsAsync_BadApiKey_ThrowsUnauthorizedAccessException()
    {
        var dataClient = new DataClient(Guid.NewGuid().ToString()[..5]);

        List<ApiResponseException> excs = new();

        ApiEventPublisher.RaiseApiResponseEventHandler += (sender, e) =>
        {
            if (e.ApiResponseException != null)
            {
                excs.Add(e.ApiResponseException);
            }
        };

        Assert.Equal(new TrendCollection(), await dataClient.GetTrendsForSymbolsAsync("AAPL"));
        Assert.True(excs.Count > 0);
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetTrendsForSymbolsAsync_BadSymbol_Empty()
    {
        var dataClient = new DataClient(apiKey);

        Assert.Equal(new TrendCollection(), await dataClient.GetTrendsForSymbolsAsync(Guid.NewGuid().ToString()[..4]));
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetTrendsForSymbolsAsync_Valid_NotEmpty()
    {
        var dataClient = new DataClient(apiKey);
        var trends = await dataClient.GetTrendsForSymbolsAsync("MSFT");

        Assert.Equal("MSFT", trends.Symbols);
        Assert.Equal("Trends", trends.Type);
        Assert.NotNull(trends.Trends);
        Assert.NotEmpty(trends.Trends);
    }

    [Fact]
    public async Task GetIposAsync_BadApiKey_ThrowsUnauthorizedAccessException()
    {
        var dataClient = new DataClient(Guid.NewGuid().ToString()[..5]);

        List<ApiResponseException> excs = new();

        ApiEventPublisher.RaiseApiResponseEventHandler += (sender, e) =>
        {
            if (e.ApiResponseException != null)
            {
                excs.Add(e.ApiResponseException);
            }
        };

        Assert.Equal(new IpoCollection(), await dataClient.GetIposAsync());
        Assert.Single(excs);
    }

    [Fact] //[Fact(Skip = "Expensive")]
    public async Task GetIposAsync_Valid_NotEmpty()
    {
        var dataClient = new DataClient(apiKey);
        var ipos = await dataClient.GetIposAsync(new DateOnly(2015, 1, 1));

        Assert.NotNull(ipos.Ipos);
        Assert.NotEmpty(ipos.Ipos);
    }
}