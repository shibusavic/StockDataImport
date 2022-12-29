using Dapper;
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
    public async Task SaveSplitsAsync(IEnumerable<Domain.Split> splits, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (splits.Any())
        {
            string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(Split));
            var daoSplits = splits.Select(s => new Split(s));


            using var connection = await GetOpenConnectionAsync(cancellationToken);
            using var transaction = connection.BeginTransaction();

            try
            {
                await connection.ExecuteAsync(sql, daoSplits, transaction);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
    }

    /// <summary>
    /// Save the dividends.
    /// </summary>
    /// <param name="symbol">The symbol for the dividends.</param>
    /// <param name="exchange">The exchange for the symbol.</param>
    /// <param name="dividends">A collection of <see cref="EodHistoricalData.Sdk.Models.Dividend"/> values.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asyncronous operation.</returns>
    public async Task SaveDividendsAsync(string symbol, string exchange, IEnumerable<EodHistoricalData.Sdk.Models.Dividend> dividends, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (dividends.Any())
        {
            string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(Dividend));
            var daoDividends = dividends.Select(d => new Dividend(symbol, exchange, d));

            using var connection = await GetOpenConnectionAsync(cancellationToken);
            using var transaction = connection.BeginTransaction();

            try
            {
                await connection.ExecuteAsync(sql, daoDividends, transaction);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        }
    }
}
