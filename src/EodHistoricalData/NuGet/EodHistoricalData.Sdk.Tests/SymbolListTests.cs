using EodHistoricalData.Sdk.Models;

namespace EodHistoricalData.Sdk.Tests
{
    public class SymbolListTests : BaseTest
    {
        [Fact]
        public async Task GetSymbolListAsync_BadApiKey_ThrowsUnauthorizedAccessException()
        {
            var dataClient = new DataClient(Guid.NewGuid().ToString()[..5]);

            List<ApiResponseException> excs = new();

            dataClient.ApiResponseExceptionEventHandler += (sender, apiResponseException, symbols) =>
            {
                excs.Add(apiResponseException);
            };

            Assert.Empty(await dataClient.GetSymbolListAsync("NYSE"));
            Assert.Single(excs);
        }

        [Fact] //[Fact(Skip = "Expensive")]
        public async Task GetSymbolListAsync_BadExchange_Empty()
        {
            var dataClient = new DataClient(apiKey);

            Assert.Empty(await dataClient.GetSymbolListAsync(Guid.NewGuid().ToString()[..4]));
        }

        [Fact] //[Fact(Skip = "Expensive")]
        public async Task GetSymbolListAsync_ExchangeCollection()
        {
            var dataClient = new DataClient(apiKey);
            var symbols = await dataClient.GetSymbolListAsync("NYSE");
            TestSymbolCollection(symbols);
        }

        [Fact] //[Fact(Skip = "Expensive")]
        public async Task GetSymbolListStringAsync_StringSerializable()
        {
            var dataClient = new DataClient(apiKey);
            var exchangesString = await dataClient.GetSymbolListStringAsync("NYSE");

            var symbols = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Symbol>>(exchangesString, serializerOptions);

            Assert.NotNull(symbols);
            if (symbols != null)
            {
                TestSymbolCollection(symbols);
            }
        }

        private static void TestSymbolCollection(IEnumerable<Symbol> symbols)
        {
            Assert.NotEmpty(symbols);

            foreach (var symbol in symbols)
            {
                Assert.NotNull(symbol.Code);
                Assert.NotNull(symbol.Name);
                Assert.NotNull(symbol.Country);
                Assert.NotNull(symbol.Exchange);
                Assert.NotNull(symbol.Currency);
                Assert.NotNull(symbol.Type);
            }
        }
    }
}