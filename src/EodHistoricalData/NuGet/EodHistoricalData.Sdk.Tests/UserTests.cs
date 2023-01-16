namespace EodHistoricalData.Sdk.Tests;

[Collection("API Tests")]
public class UserTests : BaseTest
{
    [Fact] //[Fact(Skip = "Expensive")]
    public async Task ResetUsage_Valid_NotZero()
    {
        var dataClient = new DataClient(apiKey);
        _ = await dataClient.GetExchangeListAsync();
        var (Requests, Limit) = await dataClient.GetUsageAsync();
        Assert.True(Limit > 0);
        Assert.True(Requests > 0);
    }
}