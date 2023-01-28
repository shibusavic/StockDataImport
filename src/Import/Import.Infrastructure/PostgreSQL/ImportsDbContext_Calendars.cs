using Import.Infrastructure.PostgreSQL.DataAccessObjects;

namespace Import.Infrastructure.PostgreSQL;

internal partial class ImportsDbContext
{
    public Task SaveIpos(EodHistoricalData.Sdk.Models.Calendar.IpoCollection ipoCollection,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(CalendarIpo));

        if (sql == null) { throw new Exception($"Could not create upsert for {nameof(CalendarIpo)}"); }

        var daoIpos = ipoCollection.Ipos.Select(x => new CalendarIpo(x,
            SymbolMetaDataRepository.GetFirstExchangeForSymbol(x.Symbol))).ToArray();

        return ExecuteAsync(sql, daoIpos, null, cancellationToken);
    }

    public Task SaveEarnings(EodHistoricalData.Sdk.Models.Calendar.EarningsCollection earnings,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(CalendarEarnings));

        if (sql == null) { throw new Exception($"Could not create upsert for {nameof(CalendarEarnings)}"); }

        var daoEarnings = earnings.Earnings.Select(e => new CalendarEarnings(e));

        return ExecuteAsync(sql, daoEarnings, null, cancellationToken);
    }

    public Task SaveTrends(EodHistoricalData.Sdk.Models.Calendar.TrendCollection trends,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(CalendarTrend));

        if (sql == null) { throw new Exception($"Could not create upsert for {nameof(CalendarTrend)}"); }

        var daoTrend = trends.Trends.Select(t => new CalendarTrend(t));

        return ExecuteAsync(sql, daoTrend, null, cancellationToken);
    }
}
