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

        const string logItemSql = @"
INSERT INTO public.eod_logs
(global_id, utc_timestamp, log_level, message, exception, log_scope, event_id, event_name)
VALUES
(@GlobalId, @UtcTimestamp, @LogLevel, @Message, @Exception, @Scope, @EventId, @EventName)
ON CONFLICT (global_id)
DO NOTHING
";

        const string logDataSql = @"
INSERT INTO public.eod_logs_extended
(log_id, log_key, log_value)
VALUES
(@LogId, @Key, @Value)
ON CONFLICT (log_id, log_key)
DO NOTHING
";

        object logParameters = new
        {
            logItem.GlobalId,
            logItem.UtcTimestamp,
            LogLevel = logItem.LogLevel.GetDescription(),
            logItem.Message,
            logItem.Scope,
            Exception = logItem.Exception?.ToString(),
            EventId = logItem.EventId.Id,
            EventName = logItem.EventId.Name
        };

        await retryPolicy.ExecuteAsync(async () =>
        {
            using var connection = await GetOpenConnectionAsync(cancellationToken);
            using NpgsqlTransaction transaction = await connection.BeginTransactionAsync(cancellationToken);
            try
            {
                await connection.ExecuteAsync(logItemSql, logParameters, transaction);

                if (logItem.Data.Any())
                {
                    await connection.ExecuteAsync(logDataSql, logItem.Data.Select(d => new
                    {
                        LogId = logItem.GlobalId,
                        d.Key,
                        d.Value
                    }), transaction);
                }

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

        string? sql = Shibusa.Data.PostgeSQLSqlBuilder.CreateUpsert(typeof(DataAccessObjects.ActionLog));
        var dao = new DataAccessObjects.ActionLog(actionItem);

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

    public Task SaveActionItemsAsync(IEnumerable<ActionItem> actions, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<ActionItem?> GetActionItemAsync(Guid globalId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        string sql = $@"
{Shibusa.Data.PostgeSQLSqlBuilder.CreateSelect(typeof(DataAccessObjects.ActionLog))}
WHERE global_id = @GlobalId";

        using var connection = await GetOpenConnectionAsync(cancellationToken);

        var fromDb = await connection.QueryFirstOrDefaultAsync<DataAccessObjects.ActionLog>(sql, new
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
{Shibusa.Data.PostgeSQLSqlBuilder.CreateSelect(typeof(DataAccessObjects.ActionLog))}
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

        DataAccessObjects.ActionLog[] daos = ((await connection.QueryAsync<DataAccessObjects.ActionLog>(sql, new
        {
            Statuses = statuses
        })) ?? Enumerable.Empty<DataAccessObjects.ActionLog>()).ToArray();

        await connection.CloseAsync();

        return daos.Any() ? daos.Select(d => d.ToDomainObject()!) : Enumerable.Empty<ActionItem>();
    }

    public async Task TruncateLogsAsync(string logLevel, DateTime date, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string logsSql = @"
DELETE FROM public.eod_logs WHERE log_level = @LogLevel AND utc_timestamp < @Date";
        const string extendedLogsSql = @"
DELETE FROM public.eod_logs_extended WHERE log_id NOT IN (SELECT global_id FROM public.eod_logs)";

        await ExecuteAsync(logsSql, new
        {
            LogLevel = logLevel,
            Date = date
        }, 120, cancellationToken);

        await ExecuteAsync(extendedLogsSql, null, 120, cancellationToken);
    }

    public async Task PurgeLogsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        const string logsSql = @"
DELETE FROM public.eod_logs";
        const string extendedLogsSql = @"
DELETE FROM public.eod_logs_extended";

        await ExecuteAsync(logsSql, null, 120, cancellationToken);

        await ExecuteAsync(extendedLogsSql, null, 120, cancellationToken);
    }

    public async Task PurgeActionLogsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await ExecuteAsync(@"DELETE FROM public.eod_action_logs", cancellationToken: cancellationToken);
    }
}