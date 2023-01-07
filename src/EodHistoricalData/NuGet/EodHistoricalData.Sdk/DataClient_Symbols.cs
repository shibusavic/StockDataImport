using EodHistoricalData.Sdk.Events;
using EodHistoricalData.Sdk.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EodHistoricalData.Sdk;

public sealed partial class DataClient : IDataClient
{
    private const string SymbolListSourceName = "Symbols";

    public async Task<IEnumerable<Symbol>> GetSymbolListAsync(string exchangeCode, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? json = await GetSymbolListStringAsync(exchangeCode, cancellationToken);

        var symbols = string.IsNullOrWhiteSpace(json) ? Enumerable.Empty<Symbol>()
            : JsonSerializer.Deserialize<IEnumerable<Symbol>>(json, SerializerOptions)
                ?? Enumerable.Empty<Symbol>();

        return symbols.Where(s => s.Exchange.Equals(exchangeCode, StringComparison.InvariantCultureIgnoreCase));
    }

    internal async Task<string?> GetSymbolListStringAsync(string exchangeCode, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await GetStringResponseAsync(BuildSymbolListUri(exchangeCode.ToUpper()), SymbolListSourceName, cancellationToken);
    }

    private string BuildSymbolListUri(string exchangeCode) =>
        $"{ApiService.ExchangeSymbolListUri}{exchangeCode}?{GetTokenAndFormat()}";
}
