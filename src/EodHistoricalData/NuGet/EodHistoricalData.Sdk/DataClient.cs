using EodHistoricalData.Sdk.Events;
using EodHistoricalData.Sdk.Models;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
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

    /// <summary>
    /// Makes a call to the API to get current usage stats and resets the values
    /// in <see cref="ApiService"/>.
    /// </summary>
    /// <param name="limit">The lesser value between the one received from the API and this value
    /// is the valus assigned to <see cref="ApiService.DailyLimit"/>.</param>
    /// <returns>A task representing the asyncronous object; the task contains a 
    /// tuple of (int, int) representing the current usage and daily limit.
    /// </returns>
    public async Task<(int Usage, int Limit)> ResetUsageAsync(int limit = 100_000)
    {
        var (Usage, Limit) = await GetUsageAsync();
        ApiService.Usage = Usage;
        ApiService.DailyLimit = Math.Min(Limit, limit);
        return (ApiService.Usage, ApiService.DailyLimit);
    }

    public async Task<(int Usage, int Limit)> GetUsageAsync()
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

        if (string.IsNullOrWhiteSpace(uri)) { throw new ArgumentNullException(nameof(uri)); }

        source ??= nameof(GetStringResponseAsync);

        HttpResponseMessage response = await httpClient.GetAsync(uri, cancellationToken);
        ApiService.AddCallToUsage(uri);

        if (response.IsSuccessStatusCode)
        {
            string stringResponse = await response.Content.ReadAsStringAsync(cancellationToken);

            DomainEventPublisher.RaiseApiResponseEvent(this, (int)response.StatusCode, uri, stringResponse, source);

            return stringResponse;
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

    private static string GetFormat(string format = "json") => $"fmt={format}";

    private string BuildUserUri() => $"{ApiService.UserUri}?{GetTokenAndFormat()}";
}