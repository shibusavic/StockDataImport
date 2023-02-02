using Dapper;
using EodHistoricalData.Sdk.Events;
using Import.Infrastructure.Abstractions;
using Import.Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Npgsql;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using System.Text;

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
        string sanityCheckSql = @"SELECT current_catalog";

        try
        {
            var result = QueryAsync<string>(sanityCheckSql).GetAwaiter().GetResult();
        }
        catch
        {
            throw new ArgumentException($"Unable to reconcile connection string: {connectionString}");
        }
    }

    /// <summary>
    /// Purges all tables in the imports database in the specified schema.
    /// </summary>
    /// <param name="schema">The schema, which defaults to "public."</param>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/></param>
    /// <returns>A task representing the asyncronous operation.</returns>
    internal async Task PurgeAsync(string schema = "public", CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tableNames = (await GetTableNames(schema, cancellationToken)).ToArray();

        CancellationTokenSource cts = new();

        Collection<Task> tasks = new();

        for (int i = 0; i < DaoTableNames.Keys.Count; i++)
        {
            var attr = (TableAttribute?)DaoTypes[i].GetCustomAttribute(typeof(TableAttribute), true);
            if (attr == null || attr.Schema != schema || !tableNames.Contains(attr.Name)) continue;

            string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateDelete(DaoTypes[i]);

            if (sql == null) throw new Exception($"Could not create DELETE for {DaoTypes[i].Name}");

            tasks.Add(Task.Run(() => ExecuteAsync(sql, null, 120, cancellationToken)));
        }

        try
        {
            Task.WaitAll(tasks.ToArray(), cancellationToken);
        }
        catch (AggregateException e)
        {
            StringBuilder msg = new();
            bool raisedCancellationMessage = false;

            foreach (var ie in e.InnerExceptions)
            {
                if (ie is OperationCanceledException && !raisedCancellationMessage)
                {
                    ApiEventPublisher.RaiseMessageEvent(this, "Purging of the database was cancelled.", nameof(PurgeAsync), LogLevel.Information);
                    raisedCancellationMessage = true;
                }
                else
                {
                    ApiEventPublisher.RaiseMessageEvent(this, "Purge", ie.Message, nameof(PurgeAsync), LogLevel.Information);
                    logger?.LogError(ie, "{MESSAGE}", ie.Message);
                }
            }
        }
        finally
        {
            cts.Dispose();
        }
    }

    internal Task<IEnumerable<string>> GetTableNames(string schema = "public", CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string sql = @"SELECT table_name AS Name FROM information_schema.tables WHERE table_schema = @Schema";

        return QueryAsync<string>(sql, new { Schema = schema }, cancellationToken: cancellationToken);
    }

    /// <summary>
    /// Execute the specified SQL against the database with a transaction and without retry logic.
    /// </summary>
    /// <param name="sql">The SQL to execute.</param>
    /// <param name="parameters">An optional object specifying the parameters for the <paramref name="sql"/> argument.</param>
    /// <param name="commandTimeout">An optional timeout.</param>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/></param>
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
        catch (Exception exc)
        {
            await transaction.RollbackAsync(cancellationToken);

            DomainEventPublisher.RaiseDatabaseError(this, nameof(ExecuteAsync), exc, sql, parameters);

            throw;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    /// <summary>
    /// Queries the database and returns the result.
    /// </summary>
    /// <typeparam name="T">The type of object expected from the query.</typeparam>
    /// <param name="sql">The SQL.</param>
    /// <param name="parameters">The object corresponding to SQL variables.</param>
    /// <param name="commandTimeout">An optional command timeout.</param>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/></param>
    /// <returns>A task; the task contains an IEnumerable of <typeparamref name="T"/></returns>
    internal async Task<IEnumerable<T>> QueryAsync<T>(string sql,
        object? parameters = null,
        int? commandTimeout = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(sql)) { throw new ArgumentNullException(nameof(sql)); }

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
