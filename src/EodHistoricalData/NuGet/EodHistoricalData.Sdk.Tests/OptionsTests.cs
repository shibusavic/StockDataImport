using EodHistoricalData.Sdk.Events;
using EodHistoricalData.Sdk.Models.Options;

namespace EodHistoricalData.Sdk.Tests;

[Collection("API Tests")]
public class OptionsTests : BaseTest
{
    [Fact]
    public async Task GetOptionsForSymbolAsync_BadApiKey_ThrowsUnauthorizedAccessException()
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

        Assert.Equal(OptionsCollection.Empty, await dataClient.GetOptionsForSymbolAsync("MSFT"));
        Assert.True(excs.Count > 0);
    }

    [Fact]
    public async Task GetOptionsForSymbolAsync_Valid_NotEmpty()
    {
        var dataClient = new DataClient(apiKey);

        var actual = await dataClient.GetOptionsForSymbolAsync("MSFT");

        Assert.NotEmpty(actual.Data);
    }

    [Fact]
    public async Task GetOptionsForSymbol_DateRange_NotEmpty()
    {
        var dataClient = new DataClient(apiKey);

        var actual = await dataClient.GetOptionsForSymbolAsync("MSFT", null,
            DateOnly.FromDateTime(DateTime.Now),
            DateOnly.FromDateTime(DateTime.Now.AddMonths(1)));

        Assert.NotEmpty(actual.Data);
    }
}

