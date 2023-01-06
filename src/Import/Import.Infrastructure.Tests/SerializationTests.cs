using EodHistoricalData.Sdk;
using System.Text.Json;

namespace Import.Infrastructure.Tests
{
    public class SerializationTests
    {
        [Fact]
        public void Etf_Serialization()
        {
            string json = File.ReadAllText(@"MockData/spy-fundamentals.json");
            Assert.NotNull(json);

            JsonSerializerOptions serializerOptions = JsonSerializerOptionsFactory.Default;

            var fundamentalsCollection = JsonSerializer.Deserialize<EodHistoricalData.Sdk.Models.Fundamentals.Etf.EtfFundamentalsCollection?>(
                json, serializerOptions);

            Assert.NotNull(fundamentalsCollection);

        }

        [Fact]
        public void CommonStock_Serialization()
        {
            string json = File.ReadAllText(@"MockData/aapl-fundamentals.json");
            Assert.NotNull(json);

            JsonSerializerOptions serializerOptions = JsonSerializerOptionsFactory.Default;

            var fundamentalsCollection = JsonSerializer.Deserialize<EodHistoricalData.Sdk.Models.Fundamentals.CommonStock.FundamentalsCollection?>(
                json, serializerOptions);

            Assert.NotNull(fundamentalsCollection);

        }
    }
}
