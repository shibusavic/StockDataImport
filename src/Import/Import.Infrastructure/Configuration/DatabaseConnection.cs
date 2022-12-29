using Import.Infrastructure.Abstractions;
using Shibusa.Data.Abstractions;

namespace Import.Infrastructure.Configuration;

internal class DatabaseConnection
{
    /// <summary>
    /// Gets the name of the database connection.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets the connection string.
    /// </summary>
    public string? ConnectionString { get; set; }

    /// <summary>
    /// Gets the <see cref="DatabaseEngine"/>.
    /// </summary>
    public DatabaseEngine Engine { get; set; }
}