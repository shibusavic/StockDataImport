using Dapper;
using EodHistoricalData.Sdk.Models.Options;

namespace Import.Infrastructure.PostgreSQL;

internal partial class ImportsDbContext
{
    public async Task<IEnumerable<EodHistoricalData.Sdk.Models.Symbol>> GetSymbolsWithOptionsAsync(CancellationToken cancellationToken)
    {
        string sql = $@"{Shibusa.Data.PostgeSQLSqlBuilder.CreateSelect(typeof(DataAccessObjects.Symbol))}
WHERE has_options = @HasOptions";

        using var connection = await GetOpenConnectionAsync(cancellationToken);

        return await connection.QueryAsync<EodHistoricalData.Sdk.Models.Symbol>(sql, new { HasOptions = true });
    }

    public async Task SaveOptionsAsync(OptionsCollection optionsCollection, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var symbol = optionsCollection.Code;
        var exchange = optionsCollection.Exchange;

        var options = new DataAccessObjects.Options(optionsCollection);

        List<DataAccessObjects.OptionData> optionData = new();
        List<DataAccessObjects.OptionContract> optionContracts = new();

        foreach (var d in optionsCollection.Data)
        {
            optionData.Add(new DataAccessObjects.OptionData(symbol, exchange, d));

            foreach (var contract in d.Options.Values)
            {
                foreach (var c in contract)
                {
                    optionContracts.Add(new DataAccessObjects.OptionContract(symbol, exchange, c));
                }
            }
        }

        List<Task> taskList = new()
        {
            Task.Run(() => SaveOptionsDataAsync(options,cancellationToken), cancellationToken),
            Task.Run(() => SaveOptionsDataAsync(optionData.ToArray(),cancellationToken), cancellationToken),
            Task.Run(() => SaveOptionsDataAsync(optionContracts.ToArray(),cancellationToken), cancellationToken),
        };

        Task.WaitAll(taskList.ToArray(), cancellationToken);
        await Task.CompletedTask;
    }

    private async Task SaveOptionsDataAsync<T>(T obj, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (obj == null) { return; }

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(T));

        if (sql == null) { throw new Exception($"Could not create upsert for {obj!.GetType().Name}"); }

        using var connection = await GetOpenConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(sql, obj, transaction);
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

    private async Task SaveOptionsDataAsync<T>(T[] obj, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if ((obj?.Length ?? 0) == 0) return;

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(T));

        if (sql == null) { throw new Exception($"Could not create upsert for {obj!.GetType()?.GetElementType()?.Name}"); }

        using var connection = await GetOpenConnectionAsync(cancellationToken);
        using var transaction = connection.BeginTransaction();

        try
        {
            await connection.ExecuteAsync(sql, obj, transaction);
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
