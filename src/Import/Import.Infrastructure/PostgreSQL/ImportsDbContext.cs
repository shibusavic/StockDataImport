using Dapper;
using Import.Infrastructure.Abstractions;
using Microsoft.Extensions.Logging;
using Npgsql;
using Shibusa.Data.Abstractions;

namespace Import.Infrastructure.PostgreSQL;

/// <summary>
/// Represents a database connection to a PostgreSQL database.
/// </summary>
internal partial class ImportsDbContext : BasePostgreSQLContext, IImportsDbContext
{
    /// <summary>
    /// Creates a new instance of the <see cref="ImportsDbContext"/>.
    /// </summary>
    /// <param name="connectionString">The db connection string.</param>
    /// <param name="logger">An <see cref="ILogger"/> instance.</param>
    public ImportsDbContext(string connectionString, ILogger? logger = null) : base(connectionString, logger)
    {
    }

    /// <summary>
    /// Gets the database engine (PostgreSQL).
    /// </summary>
    public override DatabaseEngine DatabaseEngine => DatabaseEngine.Postgres;

    /// <summary>
    /// Check to determine if the "On Empty Database" actions should be invoked.
    /// </summary>
    /// <returns>A task representing the asyncronous operation; the task contains a boolean indicating
    /// whether the public.symbols table is empty.</returns>
    public async Task<bool> IsDatabaseEmptyAsync()
    {
        const string sql = @"SELECT COUNT(1) FROM symbols";

        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();

        return (await connection.QuerySingleAsync<int>(sql)) == 0;
    }
}
