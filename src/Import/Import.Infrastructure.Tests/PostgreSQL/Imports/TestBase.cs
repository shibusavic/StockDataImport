using EodHistoricalData.Sdk;
using Import.Infrastructure.IntegrationTests.Fixtures;
using System.Text.Json;

namespace Import.Infrastructure.IntegrationTests.PostgreSQL.Imports;

public class TestBase : IClassFixture<DbFixture>
{
    protected readonly DbFixture fixture;
    protected readonly JsonSerializerOptions serializerOptions;
    protected readonly static Random Random = new(Guid.NewGuid().GetHashCode());

    public TestBase(DbFixture fixture)
    {
        this.fixture = fixture;

        serializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = true
        };

        serializerOptions.Converters.Add(new DateOnlyJsonConverter());
        serializerOptions.Converters.Add(new NullableDateOnlyJsonConverter());

        serializerOptions.Converters.Add(new DateTimeJsonConverter());
        serializerOptions.Converters.Add(new NullableDateTimeJsonConverter());

        serializerOptions.Converters.Add(new DoubleJsonConverter());
        serializerOptions.Converters.Add(new NullableDoubleJsonConverter());

        serializerOptions.Converters.Add(new DecimalJsonConverter());
        serializerOptions.Converters.Add(new NullableDecimalJsonConverter());

        serializerOptions.Converters.Add(new BooleanJsonConverter());
        serializerOptions.Converters.Add(new NullableBooleanJsonConverter());

        serializerOptions.Converters.Add(new LongJsonConverter());
        serializerOptions.Converters.Add(new NullableLongJsonConverter());
    }
}
