namespace EodHistoricalData.Sdk.Events;

/// <summary>
/// Represents an event args instance containing a message and an optional context.
/// </summary>
public class MessageEventArgs : DomainEventArgs
{
    /// <summary>
    /// Creates a new instance of the <see cref="MessageEventArgs"/> class with only a message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="source">The source of the message.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="message"/> is null or whitespace.</exception>
    public MessageEventArgs(string message, string source) : base(source)
    {
        Message = string.IsNullOrWhiteSpace(message) ? throw new ArgumentNullException(nameof(message)) : message;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="MessageEventArgs"/> class with a context and a message.
    /// </summary>
    /// <param name="context">The context for the message.</param>
    /// <param name="message">The message.</param>
    /// <param name="source">The source of the message.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="context"/> is null or whitespace.</exception>
    public MessageEventArgs(string context, string message, string source) : this(message, source)
    {
        Context = string.IsNullOrWhiteSpace(context) ? throw new ArgumentNullException(nameof(message)) : context;
    }

    /// <summary>
    /// Gets the message.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the context.
    /// </summary>
    public string? Context { get; }

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
