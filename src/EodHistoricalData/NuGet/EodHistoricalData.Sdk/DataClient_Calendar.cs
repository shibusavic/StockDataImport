using EodHistoricalData.Sdk.Models.Calendar;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace EodHistoricalData.Sdk;
public sealed partial class DataClient
{
    private const string CalendarSourceName = "Calendar";

    internal async Task<string?> GetEarningsForSymbolsStringAsync(string symbols,
        DateOnly? from = null, DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await GetStringResponseAsync(BuildEarningsUri(symbols, from, to), CalendarSourceName, cancellationToken);
    }

    public async Task<EarningsCollection> GetEarningsForSymbolsAsync(string symbols,
        DateOnly? from = null, DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? json = await GetEarningsForSymbolsStringAsync(symbols, from, to, cancellationToken);

        return string.IsNullOrWhiteSpace(json) ? new EarningsCollection()
            : JsonSerializer.Deserialize<EarningsCollection>(json, SerializerOptions);
    }

    public async Task<EarningsCollection> GetEarningsForSymbolsAsync(IEnumerable<string> symbols,
        DateOnly? from = null, DateOnly? to = null,
        CancellationToken cancellationToken = default) =>
        await GetEarningsForSymbolsAsync(string.Join(',', symbols.Where(s => !string.IsNullOrWhiteSpace(s))),
            from, to, cancellationToken);


    public async Task<TrendCollection> GetTrendsForSymbolsAsync(string symbols,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? json = await GetTrendsForSymbolsStringAsync(symbols, cancellationToken);

        if (string.IsNullOrWhiteSpace(json)) { return new TrendCollection(); }

        var dao = JsonSerializer.Deserialize<TrendCollectionDao>(json, SerializerOptions);

        return new TrendCollection()
        {
            Type = dao.Type,
            Description = dao.Description,
            Symbols = dao.Symbols,
            Trends = dao.Trends.First()
        };
    }

    internal async Task<string?> GetTrendsForSymbolsStringAsync(string symbols,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await GetStringResponseAsync(BuildTrendsUri(symbols), CalendarSourceName, cancellationToken);
    }

    /// <summary>
    /// Represents a collection of <see cref="Trend"/> as returned by the API.
    /// <seealso href="https://eodhistoricaldata.com/financial-apis/calendar-upcoming-earnings-ipos-and-splits/"/>
    /// </summary>
    internal struct TrendCollectionDao
    {
        public string Type;
        public string Description;
        public string Symbols;
        public Trend[][] Trends;

        public TrendCollectionDao(string type, string description, string symbols, IEnumerable<Trend[]> trends)
        {
            Type = type;
            Description = description;
            Symbols = symbols;
            Trends = trends.ToArray();
        }
    }

    internal async Task<string?> GetIposStringAsync(
        DateOnly? from = null, DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await GetStringResponseAsync(BuildIposUri(from, to), CalendarSourceName, cancellationToken);
    }

    public async Task<IpoCollection> GetIposAsync(
        DateOnly? from = null, DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? json = await GetIposStringAsync(from, to, cancellationToken);

        return string.IsNullOrWhiteSpace(json) ? new IpoCollection()
            : JsonSerializer.Deserialize<IpoCollection>(json, SerializerOptions);
    }

    // The API returns an empty array if the 'from' value is null in this instance.
    private string BuildEarningsUri(string symbols, DateOnly? from = null, DateOnly? to = null) =>
        $"{ApiService.CalendarUri}earnings?{GetTokenAndFormat()}&symbols={symbols}&{BuildFromAndTo(from ?? DateOnlyMinValue, to)}";

    private string BuildIposUri(DateOnly? from = null, DateOnly? to = null)
    {
        if (from.HasValue && to.HasValue)
        {
            var numberOfdays = to.Value.DayNumber - from.Value.DayNumber;
            if (numberOfdays < 0) { throw new ArgumentException($"{nameof(from)} should be before {nameof(to)}"); }

            var ts = new TimeSpan(days: numberOfdays, 0, 0, 0);
            if (ts.TotalDays > 365D * 10)
            {
                from = to.Value.AddYears(-10);
            }
        }

        return $"{ApiService.CalendarUri}ipos?{GetTokenAndFormat()}&{BuildFromAndTo(from, to)}";
    }

    private string BuildTrendsUri(string symbols) =>
        $"{ApiService.CalendarUri}trends?{GetTokenAndFormat()}&symbols={symbols}";
}