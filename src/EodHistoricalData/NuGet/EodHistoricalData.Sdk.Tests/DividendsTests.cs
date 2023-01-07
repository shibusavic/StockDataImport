using EodHistoricalData.Sdk.Events;
using EodHistoricalData.Sdk.Models;

namespace EodHistoricalData.Sdk.Tests
{
    public class DividendsTests : BaseTest
    {
        [Fact]
        public async Task GetDividendsForSymbolAsync_BadApiKey_ThrowsUnauthorizedAccessException()
        {
            var dataClient = new DataClient(Guid.NewGuid().ToString()[..5]);

            List<ApiResponseException> excs = new();

            DomainEventPublisher.RaiseApiResponseEventHandler += (sender, e) =>
            {
                Assert.NotNull(e.ApiResponseException);
                excs.Add(e.ApiResponseException);
            };

            Assert.Empty(await dataClient.GetDividendsForSymbolAsync("AAPL"));
            Assert.True(excs.Count > 0);
        }

        [Fact] //[Fact(Skip = "Expensive")]
        public async Task GetDividendsForSymbolAsync_BadSymbol_Empty()
        {
            var dataClient = new DataClient(apiKey);

            Assert.Empty(await dataClient.GetDividendsForSymbolAsync(Guid.NewGuid().ToString()[..4]));
        }

        [Fact]
        public async Task GetDividendsForSymbolAsync_All()
        {
            var dataClient = new DataClient(apiKey);
            var dividends = await dataClient.GetDividendsForSymbolAsync("AAPL");

            TestDividendsCollection(dividends ?? Enumerable.Empty<Dividend>());
        }

        [Fact]
        public async Task GetDividendsForSymbolAsync_From_FirstDateEqualsFrom()
        {
            DateOnly from = new(1993, 5, 28);

            var dataClient = new DataClient(apiKey);
            var dividends = await dataClient.GetDividendsForSymbolAsync("AAPL", from: new DateOnly(from.Year, from.Month, from.Day));

            Assert.NotNull(dividends);
            Assert.NotEmpty(dividends);

            if (dividends != null)
            {
                Assert.Equal(from, dividends.First().Date);
            }
        }

        [Fact]
        public async Task GetDividendsForSymbolAsync_To_LastDateEqualsTo()
        {
            DateOnly to = new(1994, 11, 18);
            var dataClient = new DataClient(apiKey);
            var history = await dataClient.GetDividendsForSymbolAsync("AAPL", to: new DateOnly(to.Year, to.Month, to.Day));

            Assert.NotNull(history);
            Assert.NotEmpty(history);

            if (history != null)
            {
                Assert.Equal(to, history.Last().Date);
            }
        }

        [Fact]
        public async Task GetDividendsForSymbolAsync_FromTo_InclusiveRange()
        {
            DateOnly from = new(1993, 5, 28);
            DateOnly to = new(1994, 11, 18);
            var dataClient = new DataClient(apiKey);
            var history = await dataClient.GetDividendsForSymbolAsync("AAPL",
                from: new DateOnly(from.Year, from.Month, from.Day),
                to: new DateOnly(to.Year, to.Month, to.Day));

            Assert.NotNull(history);
            Assert.NotEmpty(history);

            if (history != null)
            {
                Assert.Equal(from, history.First().Date);
                Assert.Equal(to, history.Last().Date);
            }
        }

        [Fact]
        public async Task GetDividendForSymbolStringAsync_StringSerializable()
        {
            var dataClient = new DataClient(apiKey);
            var dividendString = await dataClient.GetDividendsForSymbolStringAsync("AAPL");

            Assert.NotNull(dividendString);

            var dividends = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Dividend>>(dividendString, serializerOptions);

            TestDividendsCollection(dividends);
        }

        private static void TestDividendsCollection(IEnumerable<Dividend>? dividends)
        {
            Assert.NotNull(dividends);
            Assert.NotEmpty(dividends);

            if (dividends != null)
            {
                foreach (var dividend in dividends)
                {
                    Assert.NotEqual(default, dividend.Date);
                    Assert.NotEqual(default, dividend.Value);
                    Assert.NotEqual(default, dividend.UnadjustedValue);
                    Assert.NotNull(dividend.Currency);
                }
            }
        }
    }
}