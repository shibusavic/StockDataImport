﻿using Import.Infrastructure.Abstractions;
using Microsoft.Extensions.Logging;
using Shibusa.Data.Abstractions;
using Shibusa.Extensions;

namespace Import.Infrastructure;

/// <summary>
/// Represents a factory for creating <see cref="IDbContext"/> instances.
/// </summary>
internal static class DbContextFactory
{
    /// <summary>
    /// Create a new <see cref="IDbContext"/> instance.
    /// </summary>
    /// <param name="databaseEngine">The <see cref="DatabaseEngine"/>.</param>
    /// <param name="database">The <see cref="Database"/>.</param>
    /// <param name="connectionString">The connection string for the database.</param>
    /// <param name="logger">An instance of <see cref="ILogger"/>.</param>
    /// <returns>An instance of <see cref="IDbContext"/> that corresponds to the specified
    /// <paramref name="database"/>.</returns>
    public static IDbContext? Create(DatabaseEngine databaseEngine, Database database, string connectionString, ILogger? logger = null)
    {
        if (databaseEngine == DatabaseEngine.None || string.IsNullOrWhiteSpace(connectionString)) { return null; }

        return databaseEngine switch
        {
            DatabaseEngine.Postgres => database.Equals(Database.Imports)
                ? new PostgreSQL.ImportsDbContext(connectionString, logger)
                    : database.Equals(Database.Logs)
                        ? new PostgreSQL.LogsDbContext(connectionString)
                        : throw new ArgumentException($"Unknown database: {database.GetDescription()}"),
            _ => throw new ArgumentException($"Unknown database engine: {databaseEngine.GetDescription()}")
        };
    }
}