using EodHistoricalData.Sdk.Models;

namespace EodHistoricalData.Sdk.Tests
{
    public class HistoricalDataTests : BaseTest
    {
        [Fact]
        public async Task GetHistoryForSymbolAsync_BadApiKey_ThrowsUnauthorizedAccessException()
        {
            var dataClient = new DataClient(Guid.NewGuid().ToString()[..5]);

            List<ApiResponseException> excs = new();

            dataClient.ApiResponseExceptionEventHandler += (sender, apiResponseException, symbols) =>
            {
                excs.Add(apiResponseException);
            };

            Assert.Empty(await dataClient.GetPricesForSymbolAsync("AAPL"));
            Assert.Single(excs);
        }

        [Fact] //[Fact(Skip = "Expensive")]
        public async Task GetHistoryForSymbolAsync_BadSymbol_Empty()
        {
            var dataClient = new DataClient(apiKey);

            Assert.Empty(await dataClient.GetPricesForSymbolAsync(Guid.NewGuid().ToString()[..4]));
        }

        [Fact]
        public async Task GetHistoryForSymbolAsync_Daily()
        {
            var dataClient = new DataClient("OeAFFmMliFG5orCUuwAKQ8l4WWFQ67YX");
            var history = await dataClient.GetPricesForSymbolAsync("MCD");

            Assert.NotNull(history);
            Assert.NotEmpty(history);

            if (history != null)
            {
                int pos = 0;

                while (history.ElementAt(pos).Date.DayOfWeek == DayOfWeek.Friday ||
                    history.ElementAt(pos).Date.DayOfWeek == DayOfWeek.Saturday ||
                    history.ElementAt(pos).Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    pos++;
                }

                DateOnly dt1 = history.ElementAt(pos).Date;
                DateOnly dt2 = history.ElementAt(pos + 1).Date;

                Assert.Equal(1, dt2.DayNumber - dt1.DayNumber);
            }
        }

        [Fact]
        public async Task GetHistoryForSymbolAsync_Weekly()
        {
            var dataClient = new DataClient("OeAFFmMliFG5orCUuwAKQ8l4WWFQ67YX");
            var history = await dataClient.GetPricesForSymbolAsync("MCD", Constants.Period.Weekly);

            Assert.NotNull(history);
            Assert.NotEmpty(history);

            if (history != null)
            {
                int pos = 0;

                while (history.ElementAt(pos).Date.DayOfWeek != DayOfWeek.Monday)
                {
                    pos++;
                }

                DateOnly dt1 = history.ElementAt(pos).Date;
                DateOnly dt2 = history.ElementAt(pos + 1).Date;

                Assert.Equal(7, dt2.DayNumber - dt1.DayNumber);
            }
        }

        [Fact]
        public async Task GetHistoryForSymbolAsync_Monthly()
        {
            var dataClient = new DataClient("OeAFFmMliFG5orCUuwAKQ8l4WWFQ67YX");
            var history = await dataClient.GetPricesForSymbolAsync("MCD", Constants.Period.Monthly);

            Assert.NotNull(history);
            Assert.NotEmpty(history);

            if (history != null)
            {
                DateOnly dt1 = history.ElementAt(0).Date;
                DateOnly dt2 = history.ElementAt(1).Date;

                if (dt1.Month == 12) { Assert.Equal(1, dt2.Month); }
                else { Assert.Equal(1, dt2.Month - dt1.Month); }
            }
        }

        [Fact]
        public async Task GetHistoryForSymbolAsync_Ascending()
        {
            var dataClient = new DataClient("OeAFFmMliFG5orCUuwAKQ8l4WWFQ67YX");
            var history = await dataClient.GetPricesForSymbolAsync("MCD");

            Assert.NotNull(history);
            Assert.NotEmpty(history);

            if (history != null)
            {
                DateOnly dt1 = history.ElementAt(0).Date;
                DateOnly dt2 = history.ElementAt(1).Date;

                Assert.True(dt2 > dt1);
            }
        }

        [Fact]
        public async Task GetHistoryForSymbolAsync_Descending()
        {
            var dataClient = new DataClient("OeAFFmMliFG5orCUuwAKQ8l4WWFQ67YX");
            var history = await dataClient.GetPricesForSymbolAsync("MCD", order: Constants.Order.Descending);

            Assert.NotNull(history);
            Assert.NotEmpty(history);

            if (history != null)
            {
                DateOnly dt1 = history.ElementAt(0).Date;
                DateOnly dt2 = history.ElementAt(1).Date;

                Assert.True(dt1.DayNumber > dt2.DayNumber);
            }
        }

        [Fact]
        public async Task GetHistoryForSymbolAsync_From_FirstDateEqualsFrom()
        {
            DateOnly from = DateOnly.FromDateTime(DateTime.Now.AddDays(-5));

            while (from.DayOfWeek is not DayOfWeek.Tuesday and not DayOfWeek.Wednesday)
            {
                from = from.AddDays(-1);
            }

            var dataClient = new DataClient("OeAFFmMliFG5orCUuwAKQ8l4WWFQ67YX");
            var history = await dataClient.GetPricesForSymbolAsync("MCD", from: new DateOnly(from.Year, from.Month, from.Day));

            Assert.NotNull(history);
            Assert.NotEmpty(history);

            if (history != null)
            {
                Assert.Equal(from, history.First().Date);
            }
        }

        [Fact]
        public async Task GetHistoryForSymbolAsync_To_LastDateEqualsTo()
        {
            DateOnly to = DateOnly.FromDateTime(DateTime.Now.AddDays(-5));
            while (to.DayOfWeek is not DayOfWeek.Tuesday and not DayOfWeek.Wednesday)
            {
                to = to.AddDays(-1);
            }
            var dataClient = new DataClient("OeAFFmMliFG5orCUuwAKQ8l4WWFQ67YX");
            var history = await dataClient.GetPricesForSymbolAsync("MCD", to: new DateOnly(to.Year, to.Month, to.Day));

            Assert.NotNull(history);
            Assert.NotEmpty(history);

            if (history != null)
            {
                Assert.Equal(to, history.Last().Date);
            }
        }

        [Fact]
        public async Task GetHistoryForSymbolAsync_FromTo_InclusiveRange()
        {
            DateOnly from = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));
            while (from.DayOfWeek != DayOfWeek.Tuesday) { from = from.AddDays(-1); }
            DateOnly to = from.AddDays(3);
            var dataClient = new DataClient("OeAFFmMliFG5orCUuwAKQ8l4WWFQ67YX");
            var history = await dataClient.GetPricesForSymbolAsync("MCD",
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
        public async Task GetHistoryForSymbolStringAsync_StringSerializable()
        {
            var dataClient = new DataClient("OeAFFmMliFG5orCUuwAKQ8l4WWFQ67YX");
            var historyString = await dataClient.GetHistoryForSymbolStringAsync("MCD");

            var priceActions = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<PriceAction>>(historyString, serializerOptions);

            Assert.NotNull(priceActions);
            if (priceActions != null)
            {
                TestPriceActionCollection(priceActions);
            }
        }

        private static void TestPriceActionCollection(IEnumerable<PriceAction> priceActions)
        {
            Assert.NotEmpty(priceActions);

            foreach (var priceAction in priceActions)
            {
                Assert.NotEqual(default, priceAction.Date);
                Assert.NotEqual(default, priceAction.Open);
                Assert.NotEqual(default, priceAction.High);
                Assert.NotEqual(default, priceAction.Low);
                Assert.NotEqual(default, priceAction.Close);
                Assert.NotEqual(default, priceAction.Volume);
            }
        }
    }
}