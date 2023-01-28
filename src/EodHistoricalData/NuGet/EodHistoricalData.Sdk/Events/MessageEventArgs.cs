using Microsoft.Extensions.Logging;

namespace EodHistoricalData.Sdk.Events;

/// <summary>
/// Represents an event args instance containing a message and an optional context.
/// </summary>
public class MessageEventArgs : ApiEventArgs
{
    /// <summary>
    /// Creates a new instance of the <see cref="MessageEventArgs"/> class with only a message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="source">The source of the message.</param>
    /// <param name="logLevel">An optional preferred <see cref="LogLevel"/> for logging; defaults to None.</param>
    /// <remarks>The <paramref name="logLevel"/> may or may not be honored in receiving functions.</remarks>
    public MessageEventArgs(string source, string message, string[]? args = null,
        LogLevel logLevel = LogLevel.None) : base(source)
    {
        Message = string.IsNullOrWhiteSpace(message) ? throw new ArgumentNullException(nameof(message)) : message;
        LogLevel = logLevel;
        Args = args ?? Array.Empty<string>();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MessageEventArgs"/> class with a context and a message.
    /// </summary>
    /// <param name="context">The context for the message.</param>
    /// <param name="message">The message.</param>
    /// <param name="source">The source of the message.</param>
    /// <param name="logLevel">An optional preferred <see cref="LogLevel"/> for logging; defaults to None.</param>
    /// <remarks>The <paramref name="logLevel"/> may or may not be honored in consumers.</remarks>
    public MessageEventArgs(string source, string context, string message, string[]? args = null,
        LogLevel logLevel = LogLevel.None) : this(source, message, args, logLevel)
    {
        Context = string.IsNullOrWhiteSpace(context) ? throw new ArgumentNullException(nameof(context)) : context;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MessageEventArgs"/> class from an
    /// <see cref="Exception"/>.
    /// </summary>
    /// <param name="exception">The exception.</param>
    /// <param name="source">The source of the exception.</param>
    /// <param name="context">An optional string to add more context to the event.</param>
    public MessageEventArgs(Exception exception, string source, string? context = null) : this(source, exception.Message)
    {
        Exception = exception;
        LogLevel = LogLevel.Error;
        Context = context;
    }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the args for the message.
    /// </summary>
    public string[] Args { get; }

    /// <summary>
    /// Gets the context.
    /// </summary>
    public string? Context { get; }

    /// <summary>
    /// Gets the <see cref="Exception"/>, if there is one.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// Gets the preferred <see cref="LogLevel"/> for logging.
    /// </summary>
    public LogLevel LogLevel { get; }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Context)
            ? $"{Message} {base.ToString()}"
            : $"{Context} : {Message} {base.ToString()}";
    }
}
