using Shibusa.Data.Abstractions;

namespace Import.Infrastructure.Abstractions;

/// <summary>
/// Represents a generic database context.
/// </summary>
internal interface IDbContext
{
    /// <summary>
    /// Gets the connection string for the database connection.
    /// </summary>
    string ConnectionString { get; }

    /// <summary>
    /// Gets the <see cref="DatabaseEngine"/> for the database connection.
    /// </summary>
    DatabaseEngine DatabaseEngine { get; }
}