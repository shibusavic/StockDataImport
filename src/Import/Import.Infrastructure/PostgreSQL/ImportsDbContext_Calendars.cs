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

    public Task SaveEarningsAsync(EodHistoricalData.Sdk.Models.Calendar.EarningsCollection earnings,
        string[] exchanges,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(CalendarEarnings));

        if (sql == null) { throw new Exception($"Could not create UPSERT for {nameof(CalendarEarnings)}"); }

        if (earnings.Earnings != null)
        {
            List<CalendarEarnings> daos = new();

            foreach (var e in earnings.Earnings)
            {
                if (e.Code == null) { continue; }

                var existing = SymbolMetaDataRepository.Get(e.Code);

                if (existing?.Exchange != null && exchanges.Contains(existing.Exchange))
                {
                    daos.Add(new CalendarEarnings(e, existing.Exchange));
                }
            }

            if (daos.Any())
            {
                return ExecuteAsync(sql, daos, null, cancellationToken);
            }
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
