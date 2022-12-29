using Dapper;
using Import.Infrastructure.Abstractions;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Threading;

namespace Import.Infrastructure.PostgreSQL;

/// <summary>
/// Base database context for PostgreSQL.
/// </summary>
internal abstract class BasePostgreSQLContext : DbContext
{
    protected static readonly Type[] DaoTypes;
    protected static readonly Dictionary<Type, (string? Schema, string Name)> DaoTableNames;

    static BasePostgreSQLContext()
    {
        DaoTypes = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttributes(typeof(TableAttribute), true).FirstOrDefault() != null)
            .ToArray();

        DaoTableNames = new();

        foreach (var t in DaoTypes)
        {
            var attr = (TableAttribute?)t.GetCustomAttribute(typeof(TableAttribute), true);
            if (attr == null) continue;

            DaoTableNames[t] = (attr.Schema, attr.Name);
        }

    }

    protected BasePostgreSQLContext(string connectionString, ILogger? logger = null) : base(connectionString, logger)
    {
    }

    internal async Task<(string? Schema, string Name)[]> GetTableNames(string schema = "public", CancellationToken cancellationToken = default)
    {
        string tableSql = @"SELECT table_schema AS Schema, 
table_name AS Name
FROM information_schema.tables WHERE table_schema = @Schema"
        ;

        return (await QueryAsync<(string? Schema, string Name)>(tableSql, new { Schema = schema }, cancellationToken: cancellationToken)).ToArray();
    }

    public async Task PurgeAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tableNames = await GetTableNames("public", cancellationToken);

        CancellationTokenSource cts = new();

        List<Task> tasks = new(tableNames.Length);

        for (int i = 0; i < DaoTableNames.Keys.Count; i++)
        {
            var attr = (TableAttribute?)DaoTypes[i].GetCustomAttribute(typeof(TableAttribute), true);
            if (attr == null) continue;

            if (tableNames.Contains((attr.Schema, attr.Name)))
            {
                string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateDelete(DaoTypes[i]);

                if (sql != null)
                {
                    tasks.Add(Task.Factory.StartNew(() => ExecuteAsync(sql, null, 120, cts.Token), cts.Token));
                }
            }
        }

        try
        {
            Task.WaitAll(tasks.ToArray());
        }
        catch (AggregateException e)
        {
            foreach (var ie in e.InnerExceptions)
            {
                if (ie is OperationCanceledException)
                {
                    Console.WriteLine("Purging of the database was cancelled.");
                    break;
                }
                else
                {
                    logger?.LogCritical(ie, ie.Message);
                }
            }
        }
        finally
        {
            cts.Dispose();
        }
    }

    /// <summary>
    /// Execute the specified SQL against the database without retry logic.
    /// </summary>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="parameters">An optional object specifying the parameters for the <paramref name="sql"/> argument.</param>
    /// <param name="commandTimeout">An optional timeout.</param>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/>.</param>
    /// <returns>A task representing the asyncronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="sql"/> is null, empty, or whitespace.</exception>
    internal async Task ExecuteAsync(string sql, object? parameters = null,
        int? commandTimeout = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        if (string.IsNullOrWhiteSpace(sql)) { throw new ArgumentNullException(nameof(sql)); }

        using var connection = await GetOpenConnectionAsync(cancellationToken);
        using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await connection.ExecuteAsync(sql, parameters, transaction, commandTimeout);
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

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="sql"></param>
    /// <param name="parameters"></param>
    /// <param name="commandTimeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    internal async Task<IEnumerable<T>> QueryAsync<T>(string sql,
        object? parameters = null,
        int? commandTimeout = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(sql)) { throw new ArgumentNullException(nameof(sql)); }

        cancellationToken.ThrowIfCancellationRequested();

        using var connection = await GetOpenConnectionAsync(cancellationToken);

        try
        {
            return await connection.QueryAsync<T>(sql, parameters, commandTimeout: commandTimeout);
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    protected async Task<NpgsqlConnection> GetOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
