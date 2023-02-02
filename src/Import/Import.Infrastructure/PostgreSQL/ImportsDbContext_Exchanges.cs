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
    public Task SaveExchangesAsync(IEnumerable<EodHistoricalData.Sdk.Models.Exchange> exchanges,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!exchanges.Any()) { return Task.CompletedTask; }
     
        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(Exchange));

        if (sql == null) { throw new Exception($"Could not create UPSERT for {nameof(Exchange)}"); }

        var daoExchanges = exchanges.Select(x => new Exchange(x));

        return ExecuteAsync(sql, daoExchanges, null, cancellationToken);
    }
}
