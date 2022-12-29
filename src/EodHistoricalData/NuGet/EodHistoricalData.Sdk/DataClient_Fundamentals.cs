using EodHistoricalData.Sdk.Models.Fundamentals;
using EodHistoricalData.Sdk.Models.Fundamentals.CommonStock;
using EodHistoricalData.Sdk.Models.Fundamentals.Etf;
using Microsoft.Extensions.Logging;
using Shibusa.Extensions;
using System.Text.Json;

namespace EodHistoricalData.Sdk;
public sealed partial class DataClient
{
    private const string FundamentalsSourceName = "Fundamentals";

    internal async Task<string> GetFundamentalsForSymbolStringAsync(string symbol,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await GetStringResponseAsync(BuildFundamentalsUri(symbol), FundamentalsSourceName, cancellationToken);
        }
        catch (ApiResponseException apiExc)
        {
            HandleApiResponseException(apiExc, symbol.Split(','));
        }
        catch (Exception exc)
        {
            logger?.LogError(exc, "{MESSAGE}", exc.Message);
        }

        return string.Empty;
    }

    internal static (string? StringValue, SymbolType EnumValue) DetermineSymbolTypeForFundamentalsOutput(string json)
    {
        if (string.IsNullOrWhiteSpace(json)) { return (null, SymbolType.None); }

        var info = JsonSerializer.Deserialize<FundamentalsInfo>(json, SerializerOptions);

        return (info.General.Type, info.General.Type.GetEnum<SymbolType>());
    }

    internal static async Task<FundamentalsCollection> ConvertToCommonStockFundamentals(string json) =>
        string.IsNullOrWhiteSpace(json) ? FundamentalsCollection.Empty
        : await Task.FromResult(JsonSerializer.Deserialize<FundamentalsCollection>(json, SerializerOptions));

    internal static async Task<EtfFundamentalsCollection> ConvertToEtfFundamentals(string json) =>
        string.IsNullOrWhiteSpace(json) ? EtfFundamentalsCollection.Empty
        : await Task.FromResult(JsonSerializer.Deserialize<EtfFundamentalsCollection>(json, SerializerOptions));

    public async Task<T> GetFundamentalsForSymbolAsync<T>(string symbol,
        CancellationToken cancellationToken = default) where T : struct
    {
        if (string.IsNullOrWhiteSpace(symbol) || cancellationToken.IsCancellationRequested) { return default; }

        string json = await GetFundamentalsForSymbolStringAsync(symbol, cancellationToken);

        if (string.IsNullOrWhiteSpace(json)) { return default; }

        return JsonSerializer.Deserialize<T>(json, SerializerOptions);
    }

    public async Task<object> GetFundamentalsForSymbolAsync(string symbol, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(symbol) || cancellationToken.IsCancellationRequested) { return FundamentalsCollection.Empty; }

        string json = await GetFundamentalsForSymbolStringAsync(symbol, cancellationToken);

        var (StringValue, EnumValue) = DetermineSymbolTypeForFundamentalsOutput(json);

        if (EnumValue == SymbolType.CommonStock)
        {
            return JsonSerializer.Deserialize<FundamentalsCollection>(json, SerializerOptions);
        }
        if (EnumValue == SymbolType.Etf)
        {
            return JsonSerializer.Deserialize<EtfFundamentalsCollection>(json, SerializerOptions);
        }

        logger?.LogInformation("Unknown type in {FUNCTION}: {TYPE}", nameof(GetFundamentalsForSymbolAsync), StringValue);

        return FundamentalsCollection.Empty;
    }

    private string BuildFundamentalsUri(string symbol) => $"{ApiService.FundamentalsUri}{symbol}?{GetTokenAndFormat()}";
}