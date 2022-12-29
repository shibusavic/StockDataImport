using Microsoft.Extensions.Configuration;

namespace EodHistoricalData.Sdk.Tests;

public abstract class BaseTest
{
    protected readonly string apiKey = "bad key";

    protected readonly System.Text.Json.JsonSerializerOptions serializerOptions = JsonSerializerOptionsFactory.Default;

    public BaseTest()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddUserSecrets("e3b2fd6e-5168-4bea-b09d-b1336c913a75");

        IConfigurationRoot? configuration = builder.Build();
        apiKey = configuration?.GetSection("EodHistoricalDataApiKey").Value ?? apiKey;
    }
}
