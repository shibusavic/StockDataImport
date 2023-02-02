using Import.Infrastructure.PostgreSQL.DataAccessObjects;

namespace Import.Infrastructure.PostgreSQL;

internal partial class ImportsDbContext
{
    /// <summary>
    /// Save provided splits.
    /// </summary>
    /// <param name="splits">The collection of <see cref="Domain.Split"/> instances.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asyncronous operation.</returns>
    public Task SaveSplitsAsync(IEnumerable<Domain.Split> splits, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!splits.Any()) { return Task.CompletedTask; }

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(Split));

        if (sql == null) { throw new Exception($"Could not create UPSERT for {nameof(Split)}"); }

        var daoSplits = splits.Select(s => new Split(s));

        return ExecuteAsync(sql, daoSplits, null, cancellationToken);
    }

    /// <summary>
    /// Save the dividends.
    /// </summary>
    /// <param name="symbol">The symbol for the dividends.</param>
    /// <param name="exchange">The exchange for the symbol.</param>
    /// <param name="dividends">A collection of <see cref="EodHistoricalData.Sdk.Models.Dividend"/> values.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asyncronous operation.</returns>
    public Task SaveDividendsAsync(string symbol, string exchange, IEnumerable<EodHistoricalData.Sdk.Models.Dividend> dividends, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!dividends.Any()) { return Task.CompletedTask; }

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(Dividend));

        if (sql == null) { throw new Exception($"Could not create UPSERT for {nameof(Dividend)}"); }

        var daoDividends = dividends.Select(d => new Dividend(symbol, exchange, d));

        return ExecuteAsync(sql, daoDividends, null, cancellationToken);
    }
}
