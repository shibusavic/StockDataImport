using EodHistoricalData.Sdk.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EodHistoricalData.Sdk;

public sealed partial class DataClient
{
    private const string SplitSourceName = "Split";

    public async Task<IEnumerable<Split>> GetSplitsForSymbolAsync(string symbol,
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string json = await GetSplitsForSymbolStringAsync(symbol, from, to, cancellationToken);

        return string.IsNullOrWhiteSpace(json) ? Enumerable.Empty<Split>()
            : JsonSerializer.Deserialize<IEnumerable<Split>>(json, SerializerOptions)
                ?? Enumerable.Empty<Split>();
    }

    internal async Task<string> GetSplitsForSymbolStringAsync(string symbol,
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            return await GetStringResponseAsync(BuildSplitsUri(symbol, from, to), SplitSourceName, cancellationToken);
        }
        catch (ApiResponseException apiExc)
        {
            HandleApiResponseException(apiExc, new string[] { symbol });
        }
        catch (Exception exc)
        {
            logger?.LogError(exc, "{MESSAGE}", exc.Message);
        }

        return string.Empty;
    }

    private string BuildSplitsUri(string symbol,
        DateOnly? from = null,
        DateOnly? to = null) => $"{ApiService.SplitsUri}{symbol.ToUpper()}?{GetTokenAndFormat()}&{BuildFromAndTo(from, to)}";
}
