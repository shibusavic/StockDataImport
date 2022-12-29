using EodHistoricalData.Sdk.Models;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace EodHistoricalData.Sdk;

public sealed partial class DataClient
{
    private const string HistoricalDataSourceName = "Historical Data";

    public async Task<IEnumerable<PriceAction>> GetPricesForSymbolAsync(string symbol,
        string period = Constants.Period.Daily,
        string order = Constants.Order.Ascending,
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        string json = await GetHistoryForSymbolStringAsync(symbol, period, order, from, to, cancellationToken);

        return string.IsNullOrWhiteSpace(json) ? Enumerable.Empty<PriceAction>()
            : JsonSerializer.Deserialize<IEnumerable<PriceAction>>(json, SerializerOptions)
                ?? Enumerable.Empty<PriceAction>();
    }

    internal async Task<string> GetHistoryForSymbolStringAsync(string symbol,
        string period = Constants.Period.Daily,
        string order = Constants.Order.Ascending,
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetStringResponseAsync(BuildEodUri(symbol, period, order, from, to), HistoricalDataSourceName, cancellationToken);
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

    private string BuildEodUri(string symbol,
        string period = Constants.Period.Daily,
        string order = Constants.Order.Ascending,
        DateOnly? from = null,
        DateOnly? to = null)
    {
        StringBuilder uri = new($"{ApiService.EodUri}{symbol.ToUpper()}?{GetTokenAndFormat()}");

        if (!string.IsNullOrEmpty(period)) { uri.Append($"&period={period}"); }

        if (!string.IsNullOrWhiteSpace(order)) { uri.Append($"&order={order}"); }

        uri.Append($"&{BuildFromAndTo(from, to)}");

        return uri.ToString();
    }
}
