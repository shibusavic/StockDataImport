using EodHistoricalData.Sdk.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EodHistoricalData.Sdk;

public sealed partial class DataClient
{
    private const string ExchangeListSourceName = "Exchange List";

    public async Task<IEnumerable<Exchange>> GetExchangeListAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? json = await GetExchangeListStringAsync(cancellationToken);

        return string.IsNullOrWhiteSpace(json) ? Enumerable.Empty<Exchange>()
            : JsonSerializer.Deserialize<IEnumerable<Exchange>>(json, SerializerOptions)
                ?? Enumerable.Empty<Exchange>();
    }

    internal async Task<string?> GetExchangeListStringAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await GetStringResponseAsync(BuildExchangesUri(), ExchangeListSourceName, cancellationToken);
    }

    private string BuildExchangesUri() => $"{ApiService.ExchangesUri}?{GetTokenAndFormat()}";
}
