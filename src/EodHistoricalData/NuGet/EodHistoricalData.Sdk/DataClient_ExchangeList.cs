using EodHistoricalData.Sdk.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EodHistoricalData.Sdk;

public sealed partial class DataClient
{
    private const string ExchangeListSourceName = "Exchange List";

    public async Task<IEnumerable<Exchange>> GetExchangeListAsync(CancellationToken cancellationToken = default)
    {
        string json = await GetExchangeListStringAsync(cancellationToken);

        return string.IsNullOrWhiteSpace(json) ? Enumerable.Empty<Exchange>()
            : JsonSerializer.Deserialize<IEnumerable<Exchange>>(json, SerializerOptions)
                ?? Enumerable.Empty<Exchange>();
    }

    internal async Task<string> GetExchangeListStringAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetStringResponseAsync(BuildExchangesUri(), ExchangeListSourceName, cancellationToken);
        }
        catch (ApiResponseException apiExc)
        {
            HandleApiResponseException(apiExc, Array.Empty<string>());
        }
        catch (Exception exc)
        {
            logger?.LogError(exc, "{MESSAGE}", exc.Message);
        }

        return string.Empty;
    }

    private string BuildExchangesUri() => $"{ApiService.ExchangesUri}?{GetTokenAndFormat()}";
}
