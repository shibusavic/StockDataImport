using Microsoft.Extensions.Logging;

namespace Import.Infrastructure.Logging;

/// <summary>
/// Represents an <see cref="ILogger"/> implementation that writes to a PostgreSQL db.
/// </summary>
internal sealed class PostgreSqlLogger : ILogger
{
    private LogScope? scope = null;
    private readonly Func<string, LogLevel, bool>? filter;
    private readonly string categoryName;
    private readonly PostgreSqlLoggerProvider loggerProvider;

    /// <summary>
    /// Creates a new instance of the <see cref="PostgreSqlLogger"/> class.
    /// </summary>
    /// <param name="loggerProvider">A <see cref="PostgreSqlLoggerProvider"/> instance.</param>
    /// <param name="categoryName">The category name.</param>
    /// <param name="filter">Filter criteria.</param>
    public PostgreSqlLogger(PostgreSqlLoggerProvider loggerProvider,
        string categoryName,
        Func<string, LogLevel, bool>? filter = null)
    {
        this.loggerProvider = loggerProvider ?? throw new ArgumentNullException(nameof(loggerProvider));
        this.categoryName = string.IsNullOrWhiteSpace(categoryName) ? throw new ArgumentNullException(nameof(categoryName)) : categoryName;
        this.filter = filter;
    }

    /// <summary>
    /// Begin a new scope for logs.
    /// </summary>
    /// <typeparam name="TState">The type of state.</typeparam>
    /// <param name="state">The state object.</param>
    /// <returns>A new <see cref="LogScope"/> instance.</returns>
    IDisposable ILogger.BeginScope<TState>(TState state)
    {
        scope = new LogScope(state ?? new object());
        return scope;
    }

    /// <summary>
    /// Gets an indicator of whether the specified <see cref="LogLevel"/> is enabled.
    /// </summary>
    /// <param name="logLevel">The <see cref="LogLevel"/> to evaluate.</param>
    /// <returns>An indicator of whether the specified <see cref="LogLevel"/> is enabled.</returns>
    public bool IsEnabled(LogLevel logLevel) => filter == null || filter(categoryName, logLevel);

    /// <summary>
    /// Preserve a specific log entry.
    /// </summary>
    /// <typeparam name="TState">The type of log item.</typeparam>
    /// <param name="logLevel">The <see cref="LogLevel"/>.</param>
    /// <param name="eventId">The <see cref="EventId"/>.</param>
    /// <param name="state">The state to log.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="formatter">A formatter used when <paramref name="state"/> is converted to a string.</param>
    public void Log<TState>(LogLevel logLevel,
        EventId eventId,
        TState? state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel)) { return; }

        LogItem? logItem;

        if (state == null)
        {
            if (exception == null) { throw new ArgumentException($"{nameof(state)} and {nameof(exception)} can not both be null."); }

            logItem = new LogItem(logLevel, exception.Message, exception, scope?.ScopeMessage, eventId);
        }
        else
        {
            logItem = state is LogItem ? state as LogItem :
                formatter == null ? new LogItem(logLevel, state.ToString(), exception, scope?.ScopeMessage, eventId)
                : new LogItem(logLevel, formatter(state, exception), exception, scope?.ScopeMessage, eventId);
        }

        if (logItem != null)
        {
            loggerProvider.PreserveLog(logItem);
        }
    }

    /// <summary>
    /// A private class to faciliate BeginScope() in ILogger.
    /// </summary>
    private class LogScope : IDisposable
    {
        private bool disposed = false;

        /// <summary>
        /// Creates a new instance of the <see cref="LogScope"/> class.
        /// </summary>
        /// <param name="state">The scope state.</param>
        public LogScope(object? state)
        {
            ScopeMessage = state?.ToString();
        }

        /// <summary>
        /// Gets the scope message.
        /// </summary>
        public string? ScopeMessage { get; protected set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ScopeMessage = null;
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Dispose of this object.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
