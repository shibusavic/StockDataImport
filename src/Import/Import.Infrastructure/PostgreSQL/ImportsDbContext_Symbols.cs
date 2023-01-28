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
    public async Task SaveSymbolsAsync(IEnumerable<EodHistoricalData.Sdk.Models.Symbol> symbols,
        string exchange,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // had to deviate from using PostgeSQLSqlBuilder because of the COALESCE statement for has_options
        const string sql = @"
INSERT INTO public.symbols
(code,symbol,exchange,name,country,currency,type,has_options)
VALUES
(@Code,@CodeSymbol,@Exchange,@Name,@Country,@Currency,@Type,@HasOptions)
ON CONFLICT (code)
DO UPDATE
SET name = @Name,country = @Country,currency = @Currency,type = @Type,
has_options = COALESCE(@HasOptions, symbols.has_options),
utc_timestamp = CURRENT_TIMESTAMP
";

        if (symbols.Any())
        {
            foreach (var chunk in symbols.Chunk(1000))
            {
                var dao = chunk.Select(s => new Symbol(s, exchange)).ToArray();
                await ExecuteAsync(sql, dao, 120, cancellationToken);
            }
        }
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

    public async Task<IEnumerable<SymbolMetaData>> GetSymbolMetaDataAsync(CancellationToken cancellationToken = default)
    {
        const string initialSql =
@"SELECT S.code, s.symbol, S.exchange, S.utc_timestamp as LastUpdated,
COALESCE(has_options, false) as HasOptions FROM public.symbols S
WHERE NOT EXISTS (SELECT * FROM public.symbols_to_ignore WHERE symbol = code AND exchange = S.exchange)";

        const string optionsSql =
@"SELECT O.symbol, O.utc_timestamp AS LastUpdated FROM public.options O
WHERE NOT EXISTS (SELECT * FROM public.symbols_to_ignore WHERE symbol = O.symbol AND exchange = O.exchange)";

        const string incomeStatementSql =
@"SELECT C.symbol, C.exchange, I.date FROM public.company_income_statements I
JOIN public.companies C ON I.company_id = C.global_id
WHERE NOT EXISTS (SELECT * FROM public.symbols_to_ignore WHERE symbol = C.symbol AND exchange = C.exchange)";

        const string companiesSql =
@"SELECT C.symbol, C.exchange, C.utc_timestamp AS LastUpdated FROM public.companies C
WHERE NOT EXISTS (SELECT * FROM public.symbols_to_ignore WHERE symbol = C.symbol AND exchange = C.exchange)";

        const string priceSql =
@"SELECT P.symbol, P.exchange, P.close, O.start
FROM public.price_actions P
INNER JOIN 
(SELECT symbol, exchange, MAX(start) AS start
FROM public.price_actions
GROUP BY symbol, exchange) O
ON O.symbol = P.symbol AND O.exchange = P.exchange AND O.start = P.start";

        using var connection = await GetOpenConnectionAsync(cancellationToken);

        var metaData = connection.Query<SymbolMetaData>(initialSql).ToArray();
        var optionsData = (await connection.QueryAsync<(string? Symbol, DateTime? LastUpdated)>(optionsSql)).ToArray();
        var companyData = (await connection.QueryAsync<(string? Symbol, string? Exchange, DateTime? LastUpdated)>(companiesSql)).ToArray();
        var incomeData = (await connection.QueryAsync<(string? Symbol, string? Exchange, DateTime? LastDate)>(incomeStatementSql)).ToArray();
        var priceData = (await connection.QueryAsync<(string Symbol, string Exchange, decimal Close, DateTime Start)>(priceSql)).ToArray();

        if (metaData.Length > 0)
        {
            for (int i = 0; i < metaData.Length; i++)
            {
                metaData[i].LastUpdatedCompany = companyData.FirstOrDefault(o => o.Symbol == metaData[i].Symbol && o.Exchange == metaData[i].Exchange).LastUpdated;

                var lastPrice = priceData.FirstOrDefault(p => p.Symbol == metaData[i].Symbol && p.Exchange == metaData[i].Exchange);
                if (lastPrice.Close > 0)
                {
                    metaData[i].LastTrade = (lastPrice.Start, lastPrice.Close);
                }

                metaData[i].LastUpdatedIncomeStatement = incomeData.FirstOrDefault(a => a.Symbol == metaData[i].Symbol &&
                    a.Exchange == metaData[i].Exchange).LastDate;
            }
        }

        await connection.CloseAsync();

        return metaData;
    }

    /// <summary>
    /// Get the <see cref="EodHistoricalData.Sdk.Models.Symbol"/>s for an exchange.
    /// </summary>
    /// <param name="exchange">The exchange.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asyncronous operation; the task contains a collection of
    /// <see cref="EodHistoricalData.Sdk.Models.Symbol"/> values.</returns>
    public Task<IEnumerable<EodHistoricalData.Sdk.Models.Symbol>> GetSymbolsForExchangeAsync(
        string exchange, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string sql = @$"{Shibusa.Data.PostgeSQLSqlBuilder.CreateSelect(typeof(Symbol))} WHERE exchange = @Exchange";

        return QueryAsync<EodHistoricalData.Sdk.Models.Symbol>(sql, new { Exchange = exchange }, cancellationToken: cancellationToken);

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

        try
        {
            return (await connection.QueryAsync<Symbol>(
                Shibusa.Data.PostgeSQLSqlBuilder.CreateSelect(typeof(Symbol))))
                .Select(m => new EodHistoricalData.Sdk.Models.Symbol()
                {
                    Code = m.Code,
                    Name = m.Name,
                    Country = m.Country,
                    Currency = m.Currency,
                    Exchange = m.Exchange,
                    Type = m.Type
                });
        }
        finally
        {
            await connection.CloseAsync();
        }
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


        using var connection = await GetOpenConnectionAsync(cancellationToken);

        try
        {
            return await connection.QueryFirstOrDefaultAsync<EodHistoricalData.Sdk.Models.Symbol>(sql.ToString(), new
            {
                Code = symbol,
                Exchange = exchange
            });
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public Task SaveSymbolsToIgnore(IEnumerable<IgnoredSymbol> symbols, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(SymbolToIgnore));

        if (sql == null) { throw new Exception($"Could not create upsert for {nameof(SymbolToIgnore)}"); }

        return ExecuteAsync(sql, symbols.Select(s => new SymbolToIgnore(s.Symbol, s.Exchange, s.Reason)), cancellationToken: cancellationToken);
    }

    public Task SaveSymbolToIgnore(IgnoredSymbol symbol, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(SymbolToIgnore));

        if (sql == null) { throw new Exception($"Could not create upsert for {nameof(SymbolToIgnore)}"); }

        return ExecuteAsync(sql, new SymbolToIgnore(symbol.Symbol, symbol.Exchange, symbol.Reason), cancellationToken: cancellationToken);
    }

    public async Task<IEnumerable<IgnoredSymbol>> GetSymbolsToIgnoreAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string sql = @"SELECT DISTINCT symbol, exchange, reason FROM public.symbols_to_ignore";

        return (await QueryAsync<SymbolToIgnore>(sql, cancellationToken: cancellationToken))
            .Select(s => new IgnoredSymbol(s.Symbol, s.Exchange, s.Reason));
    }

}
