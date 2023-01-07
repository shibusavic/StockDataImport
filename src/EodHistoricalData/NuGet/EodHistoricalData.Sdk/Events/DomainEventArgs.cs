namespace EodHistoricalData.Sdk.Events;

/// <summary>
/// Represents the foundation of domain-level event args.
/// </summary>
public class DomainEventArgs : EventArgs
{
    /// <summary>
    /// Creates a new instance of the <see cref="DomainEventArgs"/> class.
    /// </summary>
    /// <param name="source">The source of the event.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="source"/> is null or whitespace.</exception>
    public DomainEventArgs(string source)
    {
        Source = string.IsNullOrWhiteSpace(source) ? throw new ArgumentNullException(nameof(source)) : source;
        UtcTimestamp = DateTime.UtcNow;
    }

    /// <summary>
    /// Gets the UTC <see cref="DateTime"/> of when the <see cref="DomainEventArgs"/> instance was instantiated.
    /// </summary>
    public DateTime UtcTimestamp { get; }

    /// <summary>
    /// Gets the source of the event.
    /// </summary>
    public string Source { get; }

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => $"from {Source} at {UtcTimestamp:yyyy-MM-dd HH:mm:ss}";
}
