using EodHistoricalData.Sdk.Models.Calendar;

namespace EodHistoricalData.Sdk.Tests
{
    public class CalendarTests : BaseTest
    {
        [Fact]
        public async Task GetEarningsForSymbolsAsync_BadApiKey_ThrowsUnauthorizedAccessException()
        {
            var dataClient = new DataClient(Guid.NewGuid().ToString()[..5]);

            List<ApiResponseException> excs = new();

            dataClient.ApiResponseExceptionEventHandler += (sender, apiResponseException, symbols) =>
            {
                excs.Add(apiResponseException);
            };

            Assert.Equal(EarningsCollection.Empty, await dataClient.GetEarningsForSymbolsAsync("AAPL", DateOnly.MinValue));
            Assert.Single(excs);
        }

        [Fact] //[Fact(Skip = "Expensive")]
        public async Task GetEarningsForSymbolsAsync_BadSymbol_Empty()
        {
            var dataClient = new DataClient(apiKey);

            Assert.Equal(EarningsCollection.Empty, await dataClient.GetEarningsForSymbolsAsync(Guid.NewGuid().ToString()[..4], DateOnly.MinValue));
        }

        [Fact] //[Fact(Skip = "Expensive")]
        public async Task GetEarningsForSymbolsAsync_Valid_NotEmpty()
        {
            var dataClient = new DataClient(apiKey);
            var earnings = await dataClient.GetEarningsForSymbolsAsync("MSFT", DateOnly.MinValue);

            Assert.Equal("MSFT", earnings.Symbols);
            Assert.Equal("Earnings", earnings.Type);
            Assert.NotEmpty(earnings.Earnings);
        }

        [Fact]
        public async Task GetTrendsForSymbolsAsync_BadApiKey_ThrowsUnauthorizedAccessException()
        {
            var dataClient = new DataClient(Guid.NewGuid().ToString()[..5]);

            List<ApiResponseException> excs = new();

            dataClient.ApiResponseExceptionEventHandler += (sender, apiResponseException, symbols) =>
            {
                excs.Add(apiResponseException);
            };

            Assert.Equal(TrendCollection.Empty, await dataClient.GetTrendsForSymbolsAsync("AAPL"));
            Assert.Single(excs);
        }

        [Fact] //[Fact(Skip = "Expensive")]
        public async Task GetTrendsForSymbolsAsync_BadSymbol_Empty()
        {
            var dataClient = new DataClient(apiKey);

            Assert.Equal(TrendCollection.Empty, await dataClient.GetTrendsForSymbolsAsync(Guid.NewGuid().ToString()[..4]));
        }

        [Fact] //[Fact(Skip = "Expensive")]
        public async Task GetTrendsForSymbolsAsync_Valid_NotEmpty()
        {
            var dataClient = new DataClient(apiKey);
            var trends = await dataClient.GetTrendsForSymbolsAsync("MSFT");

            Assert.Equal("MSFT", trends.Symbols);
            Assert.Equal("Trends", trends.Type);
            Assert.NotEmpty(trends.Trends);
        }

        [Fact]
        public async Task GetIposAsync_BadApiKey_ThrowsUnauthorizedAccessException()
        {
            var dataClient = new DataClient(Guid.NewGuid().ToString()[..5]);

            List<ApiResponseException> excs = new();

            dataClient.ApiResponseExceptionEventHandler += (sender, apiResponseException, symbols) =>
            {
                excs.Add(apiResponseException);
            };

            Assert.Equal(IpoCollection.Empty, await dataClient.GetIposAsync());
            Assert.Single(excs);
        }

        [Fact] //[Fact(Skip = "Expensive")]
        public async Task GetIposAsync_Valid_NotEmpty()
        {
            var dataClient = new DataClient(apiKey);
            var ipos = await dataClient.GetIposAsync(new DateOnly(2015, 1, 1));

            Assert.NotEmpty(ipos.Ipos);
        }
    }
}