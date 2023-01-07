using Dapper;
using Import.Infrastructure.Abstractions;
using Import.Infrastructure.Domain;
using Import.Infrastructure.Logging;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Polly.Retry;
using Shibusa.Data.Abstractions;
using Shibusa.Extensions;

namespace Import.Infrastructure.PostgreSQL;

internal class LogsDbContext : BasePostgreSQLContext, ILogsDbContext
{
    private readonly AsyncRetryPolicy retryPolicy = Policy.Handle<Exception>()
        .RetryAsync(3, (exception, retry, context) =>
        {
            throw new Exception("Unable to write logs to the database.", exception);
        });

    public LogsDbContext(string connectionString, ILogger? logger = null) : base(connectionString, logger)
    {
    }

    public override DatabaseEngine DatabaseEngine => DatabaseEngine.Postgres;

    public async Task SaveLogAsync(LogItem logItem, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateInsert(typeof(DataAccessObjects.Log));

        if (string.IsNullOrWhiteSpace(sql)) { throw new Exception($"Unable to create upsert for {nameof(DataAccessObjects.Log)}"); }

        await retryPolicy.ExecuteAsync(async () =>
        {
            using var connection = await GetOpenConnectionAsync(cancellationToken);
            using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);
            try
            {
                await connection.ExecuteAsync(sql, new
                {
                    logItem.GlobalId,
                    logItem.UtcTimestamp,
                    LogLevel = logItem.LogLevel.GetDescription(),
                    logItem.Message,
                    LogScope = logItem.Scope,
                    Exception = logItem.Exception?.ToString(),
                    EventId = logItem.EventId.Id,
                    EventName = logItem.EventId.Name
                }, transaction);
                await transaction.CommitAsync();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                await connection.CloseAsync();
            }
        });
    }

    public async Task SaveActionItemAsync(ActionItem actionItem, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(DataAccessObjects.ActionItem));
        var dao = new DataAccessObjects.ActionItem(actionItem);

        await retryPolicy.ExecuteAsync(async () =>
        {
            using var connection = await GetOpenConnectionAsync(cancellationToken);
            using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);

            try
            {
                await connection.ExecuteAsync(sql, dao, transaction);
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
        });
    }

    public async Task SaveActionItemsAsync(IEnumerable<ActionItem> actions, CancellationToken cancellationToken = default)
    {
        var sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(DataAccessObjects.ActionItem));

        using var connection = await GetOpenConnectionAsync(cancellationToken);
        using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await connection.ExecuteAsync(sql, actions.Select(a => new DataAccessObjects.ActionItem(a)).ToArray(), transaction);
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

    public async Task SaveApiResponseAsync(string request, string response, int statusCode, CancellationToken cancellationToken = default)
    {
        var sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateInsert(typeof(DataAccessObjects.ApiResponse));

        using var connection = await GetOpenConnectionAsync(cancellationToken);
        using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await connection.ExecuteAsync(sql, new
            {
                Request = request,
                Response = response,
                StatusCode = statusCode,
                UtcTimestamp = DateTime.UtcNow,
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

    public async Task<ActionItem?> GetActionItemAsync(Guid globalId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string sql = $@"
{Shibusa.Data.PostgeSQLSqlBuilder.CreateSelect(typeof(DataAccessObjects.ActionItem))}
WHERE global_id = @GlobalId";

        using var connection = await GetOpenConnectionAsync(cancellationToken);

        var fromDb = await connection.QueryFirstOrDefaultAsync<DataAccessObjects.ActionItem>(sql, new
        {
            GlobalId = globalId
        });

        await connection.CloseAsync();

        return fromDb.ToDomainObject();
    }

    public async Task<IEnumerable<ActionItem>> GetActionItemsByStatusAsync(ImportActionStatus status, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string sql = $@"
{Shibusa.Data.PostgeSQLSqlBuilder.CreateSelect(typeof(DataAccessObjects.ActionItem))}
WHERE status = Any(@Statuses)
";
        List<string> statuses = new();

        var possibleItems = Enum.GetValues<ImportActionStatus>()
            .Where(e => !e.Equals(ImportActionStatus.None)).ToArray();

        foreach (var item in possibleItems)
        {
            if (status.HasFlag(item))
            {
                statuses.Add(item.GetDescription());
            }
        }

        if (!statuses.Any()) { return Enumerable.Empty<ActionItem>(); }

        using var connection = await GetOpenConnectionAsync(cancellationToken);

        DataAccessObjects.ActionItem[] daos = ((await connection.QueryAsync<DataAccessObjects.ActionItem>(sql, new
        {
            Statuses = statuses
        })) ?? Enumerable.Empty<DataAccessObjects.ActionItem>()).ToArray();

        await connection.CloseAsync();

        return daos.Any() ? daos.Select(d => d.ToDomainObject()!) : Enumerable.Empty<ActionItem>();
    }

    public async Task TruncateLogsAsync(string logLevel, DateTime date, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string logsSql = @"
DELETE FROM public.logs WHERE log_level = @LogLevel AND utc_timestamp < @Date";

        await ExecuteAsync(logsSql, new
        {
            LogLevel = logLevel,
            Date = date
        }, 120, cancellationToken);
    }

    public async Task PurgeLogsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string logsSql = @"
DELETE FROM public.logs";

        await ExecuteAsync(logsSql, null, 120, cancellationToken);
    }

    public async Task PurgeActionItemsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await ExecuteAsync(@"DELETE FROM public.action_items", cancellationToken: cancellationToken);
    }
}