using EodHistoricalData.Sdk.Events;
using EodHistoricalData.Sdk.Models;

namespace EodHistoricalData.Sdk.Tests
{
    public class ExchangeListTests : BaseTest
    {
        [Fact]
        public async Task GetExchangeListAsync_BadApiKey_ThrowsUnauthorizedAccessException()
        {
            var dataClient = new DataClient(Guid.NewGuid().ToString()[..5]);

            List<ApiResponseException> excs = new();

            DomainEventPublisher.RaiseApiResponseEventHandler += (sender, e) =>
            {
                Assert.NotNull(e.ApiResponseException);
                excs.Add(e.ApiResponseException);
            };

            Assert.Empty(await dataClient.GetExchangeListAsync());
            Assert.True(excs.Count > 0);
        }

        [Fact] //[Fact(Skip = "Expensive")]
        public async Task GetExchangeListAsync_ExchangeCollection()
        {
            var dataClient = new DataClient(apiKey);
            var exchanges = await dataClient.GetExchangeListAsync();
            TestExchangeCollection(exchanges);
        }

        [Fact] //[Fact(Skip = "Expensive")]
        public async Task GetExchangeListStringAsync_StringSerializable()
        {
            var dataClient = new DataClient(apiKey);
            var exchangesString = await dataClient.GetExchangeListStringAsync();

            Assert.NotNull(exchangesString);

            var exchanges = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Exchange>>(exchangesString, serializerOptions);

            Assert.NotNull(exchanges);
            if (exchanges != null)
            {
                TestExchangeCollection(exchanges);
            }
        }

        private static void TestExchangeCollection(IEnumerable<Exchange> exchanges)
        {
            Assert.NotEmpty(exchanges);

            foreach (var exchange in exchanges)
            {
                Assert.NotNull(exchange.Name);
                Assert.NotNull(exchange.Code);
                Assert.NotNull(exchange.Country);
                Assert.NotNull(exchange.Currency);
                //Assert.NotNull(exchange.OperatingMic); // this can be null
            }
        }
    }
}