using Dapper;
using Import.Infrastructure.Tests.PostgreSQL;
using Import.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Import.Infrastructure.Tests.Fixtures;

public class DbFixture : IDisposable
{
    private bool disposedValue;
    private readonly IConfiguration configuration;
    private readonly PostgreSqlLoggerProvider provider;
    private const string LogMessage = "Test Log";

    public DbFixture()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddUserSecrets("2d1c4f12-837a-425c-ad33-c9ca12c1e76b");

        configuration = builder.Build();

        var logsConnectionString = configuration.GetConnectionString("Logs") ?? throw new Exception("Could not determine connection string for Logs.");
        var importConnectionString = configuration.GetConnectionString("Imports") ?? throw new Exception("Could not determine connection string for Imports.");

        provider = new PostgreSqlLoggerProvider(logsConnectionString);

        ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.AddProvider(provider);
        });

        Logger = loggerFactory.CreateLogger(nameof(DbFixture));

        LogsDbContext = new LogsDbContext(logsConnectionString);
        ImportDbContext = new ImportsDbContext(importConnectionString);
    }

    internal ILogger Logger { get; }

    internal LogsDbContext LogsDbContext { get; }

    internal ImportsDbContext ImportDbContext { get; }

    internal async Task<int> GetLogCountForScopeAsync(string scope)
    {
        string sql = @"SELECT COUNT(1) FROM public.logs WHERE log_scope = @Scope";

        using var connection = new NpgsqlConnection(configuration.GetConnectionString("Logs"));
        connection.Open();

        return await connection.ExecuteScalarAsync<int>(sql, new { Scope = scope });
    }

    internal async Task<string[]> GetImportDbTablesWithRecordsAsync()
    {
        var tableNames = await ImportDbContext.GetTableNames();

        List<string> tables = new();

        foreach (var (Schema, Name) in tableNames)
        {
            string name = $"{Schema}.{Name}";
            string sql = $"SELECT COUNT(*) FROM {name}";
            int count = (await ImportDbContext.QueryAsync<int>(sql)).FirstOrDefault();
            if (count > 0) { tables.Add(name); }
        }

        return tables.ToArray();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                //DeleteTestLogsAsync().GetAwaiter().GetResult();
                provider.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
