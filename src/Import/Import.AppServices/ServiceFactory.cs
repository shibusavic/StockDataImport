using Import.AppServices.Configuration;
using Import.Infrastructure;
using Import.Infrastructure.Abstractions;
using Import.Infrastructure.Configuration;
using Import.Infrastructure.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shibusa.Data.Abstractions;
using Shibusa.Extensions;

namespace Import.AppServices;

public sealed class ServiceFactory
{
    private readonly ILogger? logger;
    private readonly ILogsDbContext? logsDbContext;
    private readonly IImportsDbContext? importsDbContext;

    public ServiceFactory(IConfiguration configuration, ILogger? logger = null)
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        this.logger = logger;

        foreach (DatabaseConnection connection in GetDatabaseConnections())
        {
            if (connection.Name == Database.Imports.GetDescription() && !string.IsNullOrWhiteSpace(connection.ConnectionString))
            {
                importsDbContext = (IImportsDbContext?)DbContextFactory.Create(connection.Engine, Database.Imports, connection.ConnectionString, this.logger);
            }

            if (connection.Name == Database.Logs.GetDescription() && !string.IsNullOrWhiteSpace(connection.ConnectionString))
            {
                logsDbContext = (ILogsDbContext?)DbContextFactory.Create(connection.Engine, Database.Logs, connection.ConnectionString, logger);
            }
        }
    }

    public IConfiguration Configuration { get; }

    public ActionService GetImportActionService() =>
        logsDbContext == null ? throw new Exception($"{nameof(logsDbContext)} is not initialized")
            : importsDbContext == null ? throw new Exception($"{nameof(importsDbContext)} is not initialized")
            : new ActionService(logsDbContext, importsDbContext);

    public DataImportService GetDataImportService(string apiKey, int maxUsage = 100_000) =>
        logsDbContext == null ? throw new Exception($"{nameof(logsDbContext)} is not initialized")
            : importsDbContext == null ? throw new Exception($"{nameof(importsDbContext)} is not initialized")
            : new DataImportService(logsDbContext, importsDbContext, apiKey, maxUsage, logger);

    /// <summary>
    /// Create an <see cref="ILoggerProvider"/> instance.
    /// </summary>
    /// <param name="configuration">An instance of <see cref="IConfiguration"/>.</param>
    /// <returns>An <see cref="ILoggerProvider"/> instance.</returns>
    public static ILoggerProvider? CreateLoggerProvider(IConfiguration configuration)
    {
        string? logsEngine = configuration.GetSection($"{SectionNames.DatabaseEngines}:{Database.Logs.GetDescription()}")?.Value;

        ILoggerProvider? loggerProvider = null;
        if (!string.IsNullOrWhiteSpace(logsEngine))
        {
            DatabaseEngine engine = logsEngine.GetEnum<DatabaseEngine>();

            if (!engine.Equals(DatabaseEngine.None))
            {
                loggerProvider = engine switch
                {
                    DatabaseEngine.Postgres => new PostgreSqlLoggerProvider(configuration.GetConnectionString(Database.Logs.GetDescription())),
                    _ => null
                };
            }
        }

        return loggerProvider;
    }

    private DatabaseConnection[] GetDatabaseConnections()
    {
        List<DatabaseConnection> databaseConnections = new();

        IEnumerable<IConfigurationSection> configConnections = Configuration.GetSection("ConnectionStrings").GetChildren();

        foreach (IConfigurationSection section in configConnections)
        {
            databaseConnections.Add(new DatabaseConnection() { Name = section.Key, ConnectionString = section.Value });
        }

        IConfigurationSection dbEngineSection = Configuration.GetSection(SectionNames.DatabaseEngines);

        if (dbEngineSection != null)
        {
            foreach (IConfigurationSection section in dbEngineSection.GetChildren())
            {
                DatabaseConnection? matchingConnection = databaseConnections.FirstOrDefault(d =>
                    !string.IsNullOrWhiteSpace(d.Name) && d.Name.Equals(section.Key, StringComparison.OrdinalIgnoreCase));

                if (matchingConnection == null)
                {
                    throw new Exception("There may be a mismatch in the configuration settings between connection strings and database engines; the keys should match.");
                }

                DatabaseEngine engine = section.Value?.GetEnum<DatabaseEngine>() ?? DatabaseEngine.None;

                if (!engine.Equals(DatabaseEngine.None))
                {
                    databaseConnections[databaseConnections.IndexOf(matchingConnection)].Engine = engine;
                }
            }
        }

        return databaseConnections.ToArray();
    }
}
