using EodHistoricalData.Sdk.Models.Options;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace EodHistoricalData.Sdk;

public sealed partial class DataClient
{
    private const string OptionsDataSourceName = "Options";

    internal async Task<string> GetOptionsForSymbolStringAsync(string symbol,
        string? contractName = null,
        DateOnly? from = null,
        DateOnly? to = null,
        DateOnly? tradeDateFrom = null,
        DateOnly? tradeDateTo = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetStringResponseAsync(BuildOptionsUri(symbol, contractName, from, to, tradeDateFrom, tradeDateTo),
                OptionsDataSourceName, cancellationToken);
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

    public async Task<OptionsCollection> GetOptionsForSymbolAsync(string symbol,
        string? contractName = null,
        DateOnly? from = null,
        DateOnly? to = null,
        DateOnly? tradeDateFrom = null,
        DateOnly? tradeDateTo = null,
        CancellationToken cancellationToken = default)
    {
        string json = await GetOptionsForSymbolStringAsync(symbol, contractName, from, to, tradeDateFrom, tradeDateTo, cancellationToken);

        return string.IsNullOrWhiteSpace(json) ? OptionsCollection.Empty
            : JsonSerializer.Deserialize<OptionsCollection>(json, SerializerOptions);
    }

    private string BuildOptionsUri(string symbol,
        string? contractName = null,
        DateOnly? from = null,
        DateOnly? to = null,
        DateOnly? tradeDateFrom = null,
        DateOnly? tradeDateTo = null)
    {
        StringBuilder uri = new($"{ApiService.OptionsUri}{symbol.ToUpper()}?{GetTokenAndFormat()}");

        if (!string.IsNullOrWhiteSpace(contractName)) { uri.Append($"contract_name={contractName}"); }
        if (tradeDateFrom.HasValue) { uri.Append($"trade_date_from={tradeDateFrom:yyyy-MM-dd}"); }
        if (tradeDateTo.HasValue) { uri.Append($"trade_date_to={tradeDateTo:yyyy-MM-dd}"); }

        uri.Append($"&{BuildFromAndTo(from, to)}");

        return uri.ToString();
    }
}
