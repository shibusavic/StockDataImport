using EodHistoricalData.Sdk.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EodHistoricalData.Sdk;

public sealed partial class DataClient
{
    private const string DividendSourceName = "Dividend";

    public async Task<IEnumerable<Dividend>> GetDividendsForSymbolAsync(string symbol,
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        string json = await GetDividendsForSymbolStringAsync(symbol, from, to, cancellationToken);

        return string.IsNullOrWhiteSpace(json) ? Enumerable.Empty<Dividend>()
            : JsonSerializer.Deserialize<IEnumerable<Dividend>>(json, SerializerOptions)
                ?? Enumerable.Empty<Dividend>();
    }

    internal async Task<string> GetDividendsForSymbolStringAsync(string symbol,
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetStringResponseAsync(BuildDividendUri(symbol, from, to), DividendSourceName, cancellationToken);
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

    private string BuildDividendUri(string symbol,
    DateOnly? from = null,
    DateOnly? to = null) => $"{ApiService.DividendUri}{symbol.ToUpper()}?{GetTokenAndFormat()}&{BuildFromAndTo(from, to)}";
}
