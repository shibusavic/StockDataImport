using Dapper;
using Import.Infrastructure.PostgreSQL.DataAccessObjects;

namespace Import.Infrastructure.PostgreSQL;

internal partial class ImportsDbContext
{
    /// <summary>
    /// Save provided <see cref="EodHistoricalData.Sdk.Models.Exchange"/> values.
    /// </summary>
    /// <param name="exchanges">The collection of <see cref="EodHistoricalData.Sdk.Models.Exchange"/> values.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asyncronous operation.</returns>
    public async Task SaveExchangesAsync(IEnumerable<EodHistoricalData.Sdk.Models.Exchange> exchanges,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (exchanges.Any())
        {
            string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(Exchange));
            var daoExchanges = exchanges.Select(x => new Exchange(x));

            using var connection = await GetOpenConnectionAsync(cancellationToken);
            using var transaction = connection.BeginTransaction();

            try
            {
                await connection.ExecuteAsync(sql, daoExchanges, transaction);
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
