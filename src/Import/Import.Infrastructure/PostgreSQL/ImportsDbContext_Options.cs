using Dapper;
using EodHistoricalData.Sdk.Models.Options;

namespace Import.Infrastructure.PostgreSQL;

internal partial class ImportsDbContext
{
    public async Task<IEnumerable<EodHistoricalData.Sdk.Models.Symbol>> GetSymbolsWithOptionsAsync(CancellationToken cancellationToken)
    {
        //TODO: this is broken - need another path to this determination.
        string sql = $@"{Shibusa.Data.PostgeSQLSqlBuilder.CreateSelect(typeof(DataAccessObjects.Symbol))}";

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

        if (optionsCollection.Data != null && symbol != null && exchange != null)
        {
            foreach (var d in optionsCollection.Data)
            {
                optionData.Add(new DataAccessObjects.OptionData(symbol, exchange, d));

                if (d.Options != null)
                {
                    foreach (var contract in d.Options.Values)
                    {
                        foreach (var c in contract)
                        {
                            optionContracts.Add(new DataAccessObjects.OptionContract(symbol, exchange, c));
                        }
                    }
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

    private Task SaveOptionsDataAsync<T>(T obj, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (obj == null) { return Task.CompletedTask; }

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(T));

        if (sql == null) { throw new Exception($"Could not create UPSERT for {obj!.GetType().Name}"); }

        return ExecuteAsync(sql, obj, null, cancellationToken);
    }

    private Task SaveOptionsDataAsync<T>(T[] obj, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if ((obj?.Length ?? 0) == 0) return Task.CompletedTask;

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(T));

        if (sql == null) { throw new Exception($"Could not create UPSERT for {obj!.GetType()?.GetElementType()?.Name}"); }

        return ExecuteAsync(sql, obj, null, cancellationToken);
    }
}
