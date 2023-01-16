using EodHistoricalData.Sdk.Events;
using EodHistoricalData.Sdk.Models.Fundamentals.Etf;

namespace EodHistoricalData.Sdk.Tests;

[Collection("API Tests")]
public class EtfFundamentalsTests : BaseTest
{
    [Fact]
    public async Task GetFundamentalsForSymbolAsync_BadApiKey_ThrowsUnauthorizedAccessException()
    {
        var dataClient = new DataClient(Guid.NewGuid().ToString()[..5]);

        List<ApiResponseException> excs = new();

        DomainEventPublisher.RaiseApiResponseEventHandler += (sender, e) =>
        {
            if (e.ApiResponseException != null)
            {
                excs.Add(e.ApiResponseException);
            }
        };

        Assert.Equal(default, await dataClient.GetFundamentalsForSymbolAsync<EtfFundamentalsCollection>("VTI"));
        Assert.True(excs.Count > 0);
    }

    [Theory] // [Theory(Skip = "Expensive")]
    [InlineData("VTI")]
    public async Task GetFundamentalsForSymbolAsync_Fields_NotEmpty(string symbol)
    {
        var dataClient = new DataClient(apiKey);

        var actual = await dataClient.GetFundamentalsForSymbolAsync<EtfFundamentalsCollection>(symbol);

        Assert.NotEqual(EtfFundamentalsCollection.Empty, actual);
        Assert.Equal(symbol, actual.General.Code);
        Assert.True(actual.Technicals.TwoHundredDayMovingAverage.GetValueOrDefault() > 0M);
        Assert.False(string.IsNullOrWhiteSpace(actual.Data.CompanyName));
    }
}

