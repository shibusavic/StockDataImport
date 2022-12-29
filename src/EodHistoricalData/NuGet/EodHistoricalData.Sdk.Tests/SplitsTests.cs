using EodHistoricalData.Sdk.Models;

namespace EodHistoricalData.Sdk.Tests
{
    public class SplitsTests : BaseTest
    {
        [Fact]
        public async Task GetSplitsForSymbolAsync_BadApiKey_ThrowsUnauthorizedAccessException()
        {
            var dataClient = new DataClient(Guid.NewGuid().ToString()[..5]);

            List<ApiResponseException> excs = new();

            dataClient.ApiResponseExceptionEventHandler += (sender, apiResponseException, symbols) =>
            {
                excs.Add(apiResponseException);
            };

            Assert.Empty(await dataClient.GetSplitsForSymbolAsync("AAPL"));
            Assert.Single(excs);
        }

        [Fact] //[Fact(Skip = "Expensive")]
        public async Task GetSplitsForSymbolAsync_BadSymbol_Empty()
        {
            var dataClient = new DataClient(apiKey);

            Assert.Empty(await dataClient.GetSplitsForSymbolAsync(Guid.NewGuid().ToString()[..4]));
        }

        [Fact]
        public async Task GetSplitsForSymbolAsync_All()
        {
            var dataClient = new DataClient("OeAFFmMliFG5orCUuwAKQ8l4WWFQ67YX");
            var splits = await dataClient.GetSplitsForSymbolAsync("AAPL");

            TestSplitsCollection(splits ?? Enumerable.Empty<Split>());
        }

        [Fact]
        public async Task GetSplitsForSymbolAsync_From_FirstDateEqualsFrom()
        {
            DateOnly from = new(2000, 6, 21);

            var dataClient = new DataClient("OeAFFmMliFG5orCUuwAKQ8l4WWFQ67YX");
            var splits = await dataClient.GetSplitsForSymbolAsync("AAPL", from: new DateOnly(from.Year, from.Month, from.Day));

            Assert.NotNull(splits);
            Assert.NotEmpty(splits);

            if (splits != null)
            {
                Assert.Equal(from, splits.First().Date);
            }
        }

        [Fact]
        public async Task GetSplitsForSymbolAsync_To_LastDateEqualsTo()
        {
            DateOnly to = new(2014, 6, 9);
            var dataClient = new DataClient("OeAFFmMliFG5orCUuwAKQ8l4WWFQ67YX");
            var splits = await dataClient.GetSplitsForSymbolAsync("AAPL", to: new DateOnly(to.Year, to.Month, to.Day));

            Assert.NotNull(splits);
            Assert.NotEmpty(splits);

            if (splits != null)
            {
                Assert.Equal(to, splits.Last().Date);
            }
        }

        [Fact]
        public async Task GetSplitsForSymbolAsync_FromTo_InclusiveRange()
        {
            DateOnly from = new(2000, 6, 21);
            DateOnly to = new(2014, 6, 9);

            var dataClient = new DataClient("OeAFFmMliFG5orCUuwAKQ8l4WWFQ67YX");
            var splits = await dataClient.GetSplitsForSymbolAsync("AAPL",
                from: new DateOnly(from.Year, from.Month, from.Day),
                to: new DateOnly(to.Year, to.Month, to.Day));

            Assert.NotNull(splits);
            Assert.NotEmpty(splits);

            if (splits != null)
            {
                Assert.Equal(from, splits.First().Date);
                Assert.Equal(to, splits.Last().Date);
            }
        }

        [Fact]
        public async Task GetSplitsForSymbolStringAsync_StringSerializable()
        {
            var dataClient = new DataClient("OeAFFmMliFG5orCUuwAKQ8l4WWFQ67YX");
            var splitString = await dataClient.GetSplitsForSymbolStringAsync("AAPL");

            var splits = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<Split>>(splitString, serializerOptions);

            TestSplitsCollection(splits);
        }

        private static void TestSplitsCollection(IEnumerable<Split>? splits)
        {
            Assert.NotNull(splits);
            Assert.NotEmpty(splits);

            if (splits != null)
            {
                foreach (var split in splits)
                {
                    Assert.NotEqual(default, split.Date);
                    Assert.NotNull(split.SplitText);
                }
            }
        }
    }
}