using EodHistoricalData.Sdk.Models.Bulk;
using System.Text;
using System.Text.Json;

namespace EodHistoricalData.Sdk;

public sealed partial class DataClient
{
    private const string BulkHistoricalDataSourceName = "Bulk Historical Data";
    private const string BulkDividendsSourceName = "Bulk Dividends";
    private const string BulkSplitsSourceName = "Bulk Splits";

    public async Task<IEnumerable<BulkPriceAction>> GetBulkHistoricalDataForExchangeAsync(string exchangeCode,
        IEnumerable<string>? symbols = null, DateOnly? date = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? json = await GetBulkHistoricalDataForExchangeStringAsync(exchangeCode, symbols, date, cancellationToken);

        return string.IsNullOrWhiteSpace(json) ? Enumerable.Empty<BulkPriceAction>()
            : JsonSerializer.Deserialize<IEnumerable<BulkPriceAction>>(json, SerializerOptions)
                ?? Enumerable.Empty<BulkPriceAction>();
    }

    public async Task<IEnumerable<BulkExtendedPriceAction>> GetExtendedBulkHistoricalDataForExchangeAsync(string exchangeCode,
        IEnumerable<string>? symbols = null, DateOnly? date = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var json = await GetExtendedBulkHistoricalDataForExchangeStringAsync(exchangeCode, symbols, date, cancellationToken);

        return string.IsNullOrWhiteSpace(json) ? Enumerable.Empty<BulkExtendedPriceAction>()
            : JsonSerializer.Deserialize<IEnumerable<BulkExtendedPriceAction>>(json, SerializerOptions)
                ?? Enumerable.Empty<BulkExtendedPriceAction>();
    }

    public async Task<IEnumerable<BulkDividend>> GetBulkDividendsForExchangeAsync(string exchangeCode,
        DateOnly? date = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var json = await GetBulkDividendsForExchangeStringAsync(exchangeCode, date, cancellationToken);
        return string.IsNullOrWhiteSpace(json) ? Enumerable.Empty<BulkDividend>()
            : JsonSerializer.Deserialize<IEnumerable<BulkDividend>>(json, SerializerOptions)
            ?? Enumerable.Empty<BulkDividend>();
    }

    public async Task<IEnumerable<BulkSplit>> GetBulkSplitsForExchangeAsync(string exchangeCode,
        DateOnly? date = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var json = await GetBulkSplitsForExchangeStringAsync(exchangeCode, date, cancellationToken);
        return string.IsNullOrWhiteSpace(json) ? Enumerable.Empty<BulkSplit>()
            : JsonSerializer.Deserialize<IEnumerable<BulkSplit>>(json, SerializerOptions)
            ?? Enumerable.Empty<BulkSplit>();
    }

    internal Task<string?> GetBulkHistoricalDataForExchangeStringAsync(string exchangeCode,
        IEnumerable<string>? symbols = null,
        DateOnly? date = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return GetStringResponseAsync(BuildBulkPriceActionUri(exchangeCode, symbols, date), BulkHistoricalDataSourceName, cancellationToken);
    }

    internal Task<string?> GetExtendedBulkHistoricalDataForExchangeStringAsync(string exchangeCode,
        IEnumerable<string>? symbols = null,
        DateOnly? date = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return GetStringResponseAsync(BuildBulkPriceActionExtendedUri(exchangeCode, symbols, date), BulkHistoricalDataSourceName, cancellationToken);
    }

    internal Task<string?> GetBulkDividendsForExchangeStringAsync(string exchangeCode,
        DateOnly? date = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return GetStringResponseAsync(BuildBulkDividendsUri(exchangeCode, date), BulkDividendsSourceName, cancellationToken);
    }

    internal Task<string?> GetBulkSplitsForExchangeStringAsync(string exchangeCode,
        DateOnly? date = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return GetStringResponseAsync(BuildBulkSplitsUri(exchangeCode, date), BulkSplitsSourceName, cancellationToken);
    }

    private string BuildBulkPriceActionUri(string exchangeCode,
        IEnumerable<string>? symbols = null,
        DateOnly? date = null)
    {
        StringBuilder uri = new($"{ApiService.BulkEodUri}{exchangeCode.ToUpper()}?{GetTokenAndFormat()}");

        if (symbols?.Any() ?? false)
        {
            uri.Append($"&symbols={string.Join(',', symbols)}");
        }
        if (date.HasValue)
        {
            uri.Append($"&date={date.Value:yyyy-MM-dd}");
        }

        return uri.ToString();
    }

    private string BuildBulkPriceActionExtendedUri(string exchangeCode,
        IEnumerable<string>? symbols = null,
        DateOnly? date = null) => $"{BuildBulkPriceActionUri(exchangeCode, symbols, date)}&filter=extended";

    private string BuildBulkSplitsUri(string exchangeCode,
        DateOnly? date = null) =>
            $"{BuildBulkPriceActionUri(exchangeCode, null, date)}&type=splits";

    private string BuildBulkDividendsUri(string exchangeCode,
        DateOnly? date = null) =>
            $"{BuildBulkPriceActionUri(exchangeCode, null, date)}&type=dividends";
}
