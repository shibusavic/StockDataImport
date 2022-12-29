using Import.Infrastructure.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Import.Infrastructure.Logging;

/// <summary>
/// Represents an <see cref="ILoggerProvider"/> for PostgreSQL.
/// </summary>
public class PostgreSqlLoggerProvider : ILoggerProvider
{
    private readonly ConcurrentQueue<LogItem> logQueue;
    private bool disposedValue;

    private readonly Func<string, LogLevel, bool>? filter;
    private bool runQueue = true;

    private readonly ILogsDbContext logsDb;

    /// <summary>
    /// Creates a new instance of the <see cref="PostgreSqlLoggerProvider"/> class.
    /// </summary>
    /// <param name="connectionString">The database connection string.</param>
    /// <param name="filter">A filter by which to filter logs.</param>
    public PostgreSqlLoggerProvider(string? connectionString, Func<string, LogLevel, bool>? filter = null)
    {
        if (string.IsNullOrWhiteSpace(connectionString)) { throw new ArgumentNullException(nameof(connectionString)); }
        logsDb = new PostgreSQL.LogsDbContext(connectionString);

        this.filter = filter;

        logQueue = new ConcurrentQueue<LogItem>();

        runQueue = true;
        RunLoggerDequeue();
    }

    /// <summary>
    /// Create a new instance of <see cref="PostgreSqlLoggerProvider"/>.
    /// </summary>
    /// <param name="categoryName">The category name for the logger.</param>
    /// <returns>An instance of <see cref="PostgreSqlLoggerProvider"/>.</returns>
    public ILogger CreateLogger(string categoryName) => new PostgreSqlLogger(this, categoryName, filter);

    /// <summary>
    /// Preserve a log to the database.
    /// </summary>
    /// <param name="logItem">The <see cref="LogItem"/> to preserve.</param>
    internal void PreserveLog(LogItem logItem)
    {
        if (logItem != null)
        {
            logQueue.Enqueue(logItem);
        }
    }

    private void RunLoggerDequeue()
    {
        Task.Run(() =>
        {
            while (runQueue)
            {
                if (logQueue.TryDequeue(out LogItem? logItem))
                {
                    if (logItem != null)
                    {
                        logsDb.SaveLogAsync(logItem).GetAwaiter().GetResult();
                    }
                }
            }
        });
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                int i = 0;
                while (!logQueue.IsEmpty && i++ < 10)
                {
                    Thread.Sleep(200);
                }
                runQueue = false;
            }

            disposedValue = true;
        }
    }

    /// <summary>
    /// Dispose of this object.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}