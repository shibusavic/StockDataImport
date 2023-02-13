using Import.Infrastructure.PostgreSQL.DataAccessObjects;

namespace Import.Infrastructure.PostgreSQL;

internal partial class ImportsDbContext
{
    public Task SaveIposAsync(EodHistoricalData.Sdk.Models.Calendar.IpoCollection ipoCollection,
        string[] exchanges,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(CalendarIpo));

        if (sql == null) { throw new Exception($"Could not create UPSERT for {nameof(CalendarIpo)}"); }

        if (ipoCollection.Ipos != null)
        {
            // "Exchange" is mixed case in this instance, hence the "ToUpper()"
            var daoIpos = ipoCollection.Ipos.Where(i => exchanges.Contains(i.Exchange?.ToUpper()))
                .Select(x => new CalendarIpo(x));

            return ExecuteAsync(sql, daoIpos, null, cancellationToken);
        }

        return Task.CompletedTask;
    }

    public Task SaveEarnings(EodHistoricalData.Sdk.Models.Calendar.EarningsCollection earnings,
        string[] exchanges,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(CalendarEarnings));

        if (sql == null) { throw new Exception($"Could not create UPSERT for {nameof(CalendarEarnings)}"); }

        if (earnings.Earnings != null)
        {
            var daoEarnings = earnings.Earnings.Select(e => new CalendarEarnings(e));

            return ExecuteAsync(sql, daoEarnings, null, cancellationToken);
        }

        return Task.CompletedTask;
    }

    public Task SaveTrends(EodHistoricalData.Sdk.Models.Calendar.TrendCollection trends,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(CalendarTrend));

        if (sql == null) { throw new Exception($"Could not create UPSERT for {nameof(CalendarTrend)}"); }

        if (trends.Trends != null)
        {
            var daoTrend = trends.Trends.Select(t => new CalendarTrend(t));
            return ExecuteAsync(sql, daoTrend, null, cancellationToken);
        }

        return Task.CompletedTask;
    }
}
