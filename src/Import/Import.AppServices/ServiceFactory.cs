﻿using Import.Infrastructure;
using Import.Infrastructure.Abstractions;
using Import.Infrastructure.Configuration;
using Import.Infrastructure.Events;
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

    public async Task LoadStaticDataAsync()
    {
        if (importsDbContext != null)
        {
            DomainEventPublisher.RaiseMessageEvent(this, "Loading existing meta data.", nameof(LoadStaticDataAsync));

            SymbolMetaDataRepository.SetItems(await importsDbContext.GetSymbolMetaDataAsync());

            DomainEventPublisher.RaiseMessageEvent(this, "Loading symbols to ignore.", nameof(LoadStaticDataAsync));

            SymbolsToIgnore.SetItems(await importsDbContext.GetSymbolsToIgnoreAsync());
        }
    }

    public Task<IEnumerable<SymbolMetaData>> GetMetaData(CancellationToken cancellationToken = default)
    {
        return importsDbContext?.CreateSymbolMetaDataAsync(cancellationToken) ?? Task.FromResult(Enumerable.Empty<SymbolMetaData>());
    }

    public ActionService CreateImportActionService() =>
            importsDbContext == null
            ? throw new Exception($"{nameof(importsDbContext)} is not initialized")
            : new ActionService(importsDbContext);

    public DataImportService CreateDataImportService(string apiKey, int maxUsage = 100_000) =>
        logsDbContext == null ? throw new Exception($"{nameof(logsDbContext)} is not initialized")
            : importsDbContext == null
            ? throw new Exception($"{nameof(importsDbContext)} is not initialized")
            : new DataImportService(logsDbContext, importsDbContext, apiKey, maxUsage);

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
        // Fetch the connection strings from the configuration and add each one to an array of DatabaseConnection objects.

        var configConnections = Configuration.GetSection("ConnectionStrings").GetChildren().ToArray();

        DatabaseConnection[] dbConnections = new DatabaseConnection[configConnections.Length];

        for (int i = 0; i < configConnections.Length; i++)
        {
            dbConnections[i] = new DatabaseConnection() { Name = configConnections[i].Key, ConnectionString = configConnections[i].Value };
        }

        // If we have a db engine section in our configuration,
        // for each db specified (the engines are keyed by the connection string name):
        // if we can't find the db connection, throw an exception,
        // if we find a match, then add the engine to the DatabaseConnection object.
        // This gives us a complete picture of engine and connection string.
        IConfigurationSection dbEngineSection = Configuration.GetSection(SectionNames.DatabaseEngines);

        if (dbEngineSection != null)
        {
            foreach (IConfigurationSection section in dbEngineSection.GetChildren())
            {
                DatabaseConnection? matchingConnection = dbConnections.FirstOrDefault(d =>
                    !string.IsNullOrWhiteSpace(d.Name) && d.Name.Equals(section.Key, StringComparison.OrdinalIgnoreCase));

#pragma warning disable IDE0270 // Use coalesce expression
                if (matchingConnection == null)
                {
                    throw new Exception("There may be a mismatch in the configuration settings between connection strings and database engines; the keys should match.");
                    /* 
                     * If the above error is being thrown, it's because there's a mismatch between keys between
                     * DatabaseEngines and ConnectionStrings keys in your configuration files.
                     * 
                     * The final config should look like:
                     * 
                     * {
                     *   "DatabaseEngines": {
                     *     "Logs": "PostgreSQL",
                     *     "Imports":  "PostgreSQL"
                     *   }
                     *   "ConnectionStrings": {
                     *     "Logs": "User ID=postgres;Password=yourPassword;Host=127.0.0.1;Port=5432;Database=eod_logs;",
                     *     "Imports": "User ID=postgres;Password=yourPassword;Host=127.0.0.1;Port=5432;Database=eod_imports;"
                     *   },
                     *   "EodHistoricalDataApiKey": "your key"
                     * }
                     * 
                     * Note the shared keys between DatabaseEngines and ConnectionStrings.
                     * 
                     * In practice, these values are distributed between your appsettings.json and your secrets.json file
                     * (or however you choose manage your secrets).
                     * 
                     */
                }
#pragma warning restore IDE0270 // Use coalesce expression

                DatabaseEngine engine = section.Value?.GetEnum<DatabaseEngine>() ?? DatabaseEngine.None;

                if (!engine.Equals(DatabaseEngine.None))
                {
                    matchingConnection.Engine = engine;
                }
            }
        }

        return dbConnections;
    }
}
