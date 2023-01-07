using EodHistoricalData.Sdk.Events;
using EodHistoricalData.Sdk.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EodHistoricalData.Sdk;

/// <summary>
/// Represents an EodHistoricalData.com SDK.
/// </summary>
public sealed partial class DataClient
{
    private readonly ILogger? logger;
    private readonly string apiKey;
    private static readonly HttpClient httpClient;

    private static readonly DateOnly DateOnlyMinValue = new(1900, 1, 1);

    private static readonly JsonSerializerOptions SerializerOptions = JsonSerializerOptionsFactory.Default;

    static DataClient()
    {
        httpClient = new();
    }

    /// <summary>
    /// Creates a new instance of <see cref="DataClient"/>.
    /// </summary>
    /// <param name="apiKey">The API key to be used on all requests.</param>
    /// <param name="logger">An <see cref="ILogger"/> instance.</param>
    public DataClient(string apiKey, ILogger? logger = null)
    {
        this.apiKey = apiKey;
        this.logger = logger;
    }

    public async Task<(int Requests, int Limit)> ResetUsageAsync(int limit = 100_000)
    {
        var (Requests, Limit) = await GetUsageAsync();
        ApiService.Usage = Requests;
        ApiService.DailyLimit = Math.Min(Limit, limit);
        return (ApiService.Usage, ApiService.DailyLimit);
    }

    public async Task<(int Requests, int Limit)> GetUsageAsync()
    {
        string? json = await GetStringResponseAsync(BuildUserUri(), nameof(GetUsageAsync));

        if (string.IsNullOrWhiteSpace(json)) { return (0, 0); }

        var user = JsonSerializer.Deserialize<User>(json, SerializerOptions);

        return (user.ApiRequests, user.DailyRateLimit);
    }

    private async Task<string?> GetStringResponseAsync(string uri,
        string? source = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        source ??= nameof(GetStringResponseAsync);

        if (string.IsNullOrWhiteSpace(uri)) { throw new ArgumentNullException(nameof(uri)); }

        HttpResponseMessage response = await httpClient.GetAsync(uri, cancellationToken);
        ApiService.AddCall(uri);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        DomainEventPublisher.RaiseApiResponseEvent(this, (int)response.StatusCode, uri,
            new ApiResponseException(uri, response, source), source);

        return null;
    }

    private static string BuildFromAndTo(DateOnly? from = null, DateOnly? to = null)
    {
        List<string> results = new();
        if (from.HasValue)
        {
            results.Add($"from={from.Value:yyyy-MM-dd}");
        }

        if (to.HasValue)
        {
            results.Add($"to={to.Value:yyyy-MM-dd}");
        }

        return string.Join('&', results);
    }

    private string GetTokenAndFormat(string format = "json") => $"{GetToken()}&{GetFormat(format)}";

    private string GetToken() => $"api_token={apiKey}";

    private string GetFormat(string format = "json") => $"fmt={format}";

    //private void HandleApiResponseException(ApiResponseException exc, string[] symbols)
    //{
    //    ApiResponseExceptionEventHandler?.Invoke(this, exc, symbols);
    //}

    private string BuildUserUri() => $"{ApiService.UserUri}?{GetTokenAndFormat()}";
}