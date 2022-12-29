using Dapper;
using Import.Infrastructure.PostgreSQL.DataAccessObjects;

namespace Import.Infrastructure.PostgreSQL;

internal partial class ImportsDbContext
{
    /// <summary>
    /// Save price actions.
    /// </summary>
    /// <param name="symbol">The symbol for the price actions.</param>
    /// <param name="exchange">The exchange for the symbol.</param>
    /// <param name="priceActions">A collection of 
    /// <see cref="EodHistoricalData.Sdk.Models.PriceAction"/> values.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asyncronous operation.</returns>
    public async Task SavePriceActionsAsync(string symbol, string exchange,
        IEnumerable<EodHistoricalData.Sdk.Models.PriceAction> priceActions,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (priceActions.Any())
        {
            string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(PriceAction));
            var daoPriceActions = priceActions.Select(p => new PriceAction(symbol, exchange, p));

            using var connection = await GetOpenConnectionAsync(cancellationToken);
            using var transaction = connection.BeginTransaction();

            try
            {
                await connection.ExecuteAsync(sql, daoPriceActions, transaction);
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
