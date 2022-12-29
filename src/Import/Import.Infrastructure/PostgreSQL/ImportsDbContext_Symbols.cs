using Dapper;
using Import.Infrastructure.PostgreSQL.DataAccessObjects;
using Npgsql;
using System.Text;

namespace Import.Infrastructure.PostgreSQL;

internal partial class ImportsDbContext
{
    /// <summary>
    /// Save symbols to the data store.
    /// </summary>
    /// <param name="symbols">Symbols to preserve.</param>
    /// <returns>A task representing the asyncronous operation.</returns>
    public Task SaveSymbolsAsync(IEnumerable<EodHistoricalData.Sdk.Models.Symbol> symbols, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // had to deviate from using SqlBuilder because of the COALESCE statement.
        const string sql = @"
INSERT INTO public.symbols
(code,exchange,name,country,currency,type,has_options)
VALUES
(@Code,@Exchange,@Name,@Country,@Currency,@Type,@HasOptions)
ON CONFLICT (code,exchange)
DO UPDATE
SET name = @Name,country = @Country,currency = @Currency,type = @Type,
has_options = COALESCE(@HasOptions, symbols.has_options),
utc_timestamp = CURRENT_TIMESTAMP
";

        return symbols.Any()
            ? ExecuteAsync(sql, symbols.Select(s => new Symbol(s)), null, cancellationToken)
            : Task.CompletedTask;
    }

    /// <summary>
    /// Sets the selected symbols as optionable.
    /// </summary>
    /// <param name="symbols">The collection of symbols that are optionable.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asyncronous operation.</returns>
    public async Task SetOptionableOnSymbolsAsync(IEnumerable<string> symbols, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string sql = @"
UPDATE public.symbols SET has_options = @HasOptions WHERE code = Any(@Codes)";

        if (symbols.Any())
        {
            using var connection = await GetOpenConnectionAsync(cancellationToken);
            using var transaction = connection.BeginTransaction();

            try
            {
                await connection.ExecuteAsync(sql, new
                {
                    HasOptions = true,
                    Codes = symbols.ToArray()
                }, transaction);
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
    /// Get the <see cref="EodHistoricalData.Sdk.Models.Symbol"/>s for an exchange.
    /// </summary>
    /// <param name="exchange">The exchange.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asyncronous operation; the task contains a collection of
    /// <see cref="EodHistoricalData.Sdk.Models.Symbol"/> values.</returns>
    public async Task<IEnumerable<EodHistoricalData.Sdk.Models.Symbol>> GetSymbolsForExchangeAsync(
        string exchange, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);

        string sql = @$"{Shibusa.Data.PostgeSQLSqlBuilder.CreateSelect(typeof(Symbol))} WHERE exchange = @Exchange";

        return await connection.QueryAsync<EodHistoricalData.Sdk.Models.Symbol>(sql, new { Exchange = exchange });
    }

    /// <summary>
    /// Get all symbols.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asyncronous operation; the task contains a collection of
    /// <see cref="EodHistoricalData.Sdk.Models.Symbol"/> values.</returns>
    public async Task<IEnumerable<EodHistoricalData.Sdk.Models.Symbol>> GetAllSymbolsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);

        return await connection.QueryAsync<EodHistoricalData.Sdk.Models.Symbol>(Shibusa.Data.PostgeSQLSqlBuilder.CreateSelect(typeof(Symbol)));
    }

    /// <summary>
    /// Get a specific <see cref="EodHistoricalData.Sdk.Models.Symbol"/>.
    /// </summary>
    /// <param name="symbol">The symbol (code).</param>
    /// <param name="exchange">The exchange.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asyncronous operation; the task contains a
    /// <see cref="EodHistoricalData.Sdk.Models.Symbol"/> value.</returns>
    public async Task<EodHistoricalData.Sdk.Models.Symbol> GetSymbolAsync(string symbol, string? exchange = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        StringBuilder sql = new(Shibusa.Data.PostgeSQLSqlBuilder.CreateSelect(typeof(Symbol))
            ?? throw new ArgumentException($"Could not construct select statement for '{symbol}' in {nameof(GetSymbolAsync)}"));

        sql.Append(@"
WHERE code = @Code
");

        if (exchange != null)
        {
            sql.Append(@"AND exchange = @Exchange");
        }

        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);

        return await connection.QueryFirstOrDefaultAsync<EodHistoricalData.Sdk.Models.Symbol>(sql.ToString(), new
        {
            Code = symbol,
            Exchange = exchange
        });
    }
}
