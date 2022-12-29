using Microsoft.Extensions.Logging;

namespace Import.Infrastructure.Logging;

/// <summary>
/// Represents a log item.
/// </summary>
internal class LogItem
{
    /// <summary>
    /// Creates a new instance of the <see cref="LogItem"/> class.
    /// </summary>
    /// <param name="logLevel">The log level for the log item.</param>
    /// <param name="eventId">The event id for the log item.</param>
    /// <param name="message">The log item's message.</param>
    /// <param name="exception">The log item's exception.</param>
    /// <param name="scope">The scope of the log item.</param>
    /// <param name="data">A dictionary of additional data points to log.</param>
    public LogItem(LogLevel logLevel,
        string? message = null,
        Exception? exception = null,
        string? scope = null,
        EventId eventId = default,
        IDictionary<string, string>? data = null)
        : this(Guid.NewGuid(), logLevel, eventId,
             message, exception, scope, DateTime.UtcNow, data)
    {
    }

    internal LogItem(Guid globalId,
        LogLevel logLevel,
        EventId eventId,
        string? message,
        Exception? exception,
        string? scope,
        DateTime utcTimestamp,
        IDictionary<string, string>? data)
    {
        if (string.IsNullOrWhiteSpace(message) && exception == null && eventId == default)
        {
            throw new ArgumentException($"Either {nameof(message)}, {nameof(exception)}, or {nameof(eventId)} is required.");
        }

        GlobalId = globalId;
        LogLevel = logLevel;
        EventId = eventId;
        Message = message;
        Exception = exception;
        Scope = scope;
        UtcTimestamp = utcTimestamp;
        Data = new Dictionary<string, string>(data ?? new Dictionary<string, string>());
    }

    /// <summary>
    /// Gets the globally uunique identifier for this log item.
    /// </summary>
    public Guid GlobalId { get; }

    /// <summary>
    /// Gets the item's log level.
    /// </summary>
    public LogLevel LogLevel { get; }

    /// <summary>
    /// Gets the log item's <see cref="EventId"/>.
    /// </summary>
    public EventId EventId { get; }

    /// <summary>
    /// Gets the log item's message.
    /// </summary>
    public string? Message { get; }

    /// <summary>
    /// Gets the log item's <see cref="Exception"/>.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets the log item's scope.
    /// </summary>
    public string? Scope { get; }

    /// <summary>
    /// Gets the UTC timestamp of the creation of the log item.
    /// </summary>
    public DateTime UtcTimestamp { get; }

    /// <summary>
    /// Gets a dictionary of information attached to this log item.
    /// </summary>
    public IDictionary<string, string> Data { get; }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return Exception?.Message ?? Message ?? "Log Item";
    }
}
