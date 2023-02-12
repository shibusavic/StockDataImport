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
    public Task SavePriceActionsAsync(string symbol, string exchange,
        IEnumerable<EodHistoricalData.Sdk.Models.PriceAction> priceActions,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        symbol = Utility.ParseSymbolCode(symbol).Symbol ?? throw new ArgumentException($"Could not parse symbol from '{symbol}'");

        if (!priceActions.Any()) { return Task.CompletedTask; }

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(PriceAction));

        if (sql == null) { throw new Exception($"Could not create UPSERT for {nameof(PriceAction)}"); }

        var daoPriceActions = priceActions.Select(p => new PriceAction(symbol, exchange, p)).ToArray();

        return ExecuteAsync(sql, daoPriceActions, null, cancellationToken);
    }

    public Task SaveBulkPriceActionsAsync(IEnumerable<EodHistoricalData.Sdk.Models.Bulk.BulkPriceAction> priceActionModels,
        string exchange,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var priceActions = priceActionModels.ToArray();

        if (!priceActions.Any()) { return Task.CompletedTask; }

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(PriceAction));

        if (sql == null) { throw new Exception($"Could not create UPSERT for {nameof(PriceAction)}"); }

        var dao = priceActions.Select(p => new PriceAction(p, exchange));

        return ExecuteAsync(sql, dao, cancellationToken: cancellationToken);
    }
}
