﻿using EodHistoricalData.Sdk;
using EodHistoricalData.Sdk.Events;
using Import.AppServices;
using Import.AppServices.Configuration;
using Import.Infrastructure;
using Import.Infrastructure.Domain;
using Import.Infrastructure.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shibusa.Extensions;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using static Import.Infrastructure.Configuration.Constants;

Stopwatch timer = Stopwatch.StartNew();

string NL = Environment.NewLine;
int exitCode = -1;

const string DefaultConfigFileName = "config.yml";
const string sourceName = "import";

int maxParallel = 5;

ILoggerProvider? loggerProvider = null;
ServiceFactory serviceFactory;
ILogger? logger = null;
IConfiguration configuration;

DataImportService? dataImportService = null;
ImportConfiguration importConfiguration = new();
FileInfo configFileInfo = new(DefaultConfigFileName);

bool dryRun = false;
string? apiKey = null;
bool showHelp = false;
bool verbose = false;

HandleArguments(args);

var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;

try
{
    if (showHelp)
    {
        ShowHelp();
    }
    else
    {
        if (dryRun)
        {
            verbose = true;
            Communicate("DRY RUN", true, true);
        }

        ApiEventPublisher.RaiseMessageEventHandler += EventPublisher_RaiseMessageEventHandler;
        ApiEventPublisher.RaiseApiLimitReachedEventHandler += EventPublisher_RaiseApiLimitReachedEventHandler;

        DomainEventPublisher.DatabaseErrorHander += EventPublisher_DatabaseErrorHander;
        DomainEventPublisher.RaiseMessageEventHandler += EventPublisher_RaiseMessageEventHandler;

        await ConfigureAsync();

        string welcomeMsg = dryRun ? "Dry Run (api credit cost estimation)" : "Configuration complete.";
        DomainEventPublisher.RaiseMessageEvent(null, welcomeMsg, sourceName);

        using var cycle = dataImportService!.GetImportCycle(importConfiguration, new DirectoryInfo("cycles"));

        if (!(cycle?.Actions.Any() ?? false))
        {
            throw new Exception($"No actions were discovered with the provided configuration.");
        }

        if (dryRun)
        {
            ShowActionBlocks(cycle);
        }
        else
        {
            DomainEventPublisher.RaiseMessageEvent(null, $"Cycle created: {cycle.Id}", sourceName, LogLevel.Information);

            var purgeActions = cycle.Actions.Where(a => a.ActionName.Equals(ActionNames.Purge) &&
                a.Status == Import.Infrastructure.Abstractions.ImportActionStatus.NotStarted).ToArray();

            if (purgeActions.Any())
            {
                DomainEventPublisher.RaiseMessageEvent(null, $"{purgeActions.Length} purge actions found.", sourceName);

                await Parallel.ForEachAsync(purgeActions,
                    new ParallelOptions()
                    {
                        CancellationToken = cancellationToken,
                        MaxDegreeOfParallelism = maxParallel
                    }, async (action, ct) =>
                {
                    try
                    {
                        action.Start();

                        Task t = dataImportService.PurgeDataAsync(action.TargetName, cycle, ct);
                        DomainEventPublisher.RaiseMessageEvent(null, $"Purging {action.TargetName}", sourceName);
                        await t;
                        DomainEventPublisher.RaiseMessageEvent(null, $"Purging of {action.TargetName} complete.", sourceName);

                        if (action.TargetName.Equals(PurgeName.Imports))
                        {
                            // Meta data may have changed as a result of the purge.
                            t = dataImportService.ResetMetaDataRepositoryAsync(cancellationToken);
                            DomainEventPublisher.RaiseMessageEvent(null, $"Resetting meta data.", sourceName);
                            await t;
                        }

                        action.Complete();
                    }
                    catch (Exception exc)
                    {
                        action.Error(exc);
                    }
                });
            }

            ActionItem[] symbolActions = cycle.Actions.Where(a => a.ActionName.Equals(ActionNames.Import) &&
                a.Status == Import.Infrastructure.Abstractions.ImportActionStatus.NotStarted &&
                a.TargetDataType == DataTypes.Symbols).ToArray();

            if (symbolActions.Any())
            {
                foreach (var action in symbolActions)
                {
                    await dataImportService.ImportDataAsync(action, importConfiguration, cancellationToken);
                }
            }

            ActionItem[] importActions = cycle.Actions.Where(a => a.ActionName.Equals(ActionNames.Import) &&
                a.Status == Import.Infrastructure.Abstractions.ImportActionStatus.NotStarted)
                .Except(symbolActions).ToArray();

            if (importActions.Any())
            {
                DomainEventPublisher.RaiseMessageEvent(null, $"{importActions.Length} import actions found.", sourceName);

#if DEBUG
                var startCount = SymbolMetaDataRepository.Count();
                var startLastUpdate = startCount == 0 ? DateTime.MinValue : SymbolMetaDataRepository.GetAll().Select(s => s.LastUpdated).Max();
#endif

                var priorities = importActions.Select(a => a.Priority).Distinct().OrderBy(a => a).ToArray();

                for (int i = 0; i < priorities.Length; i++)
                {
                    var actionsToRun = importActions.Where(a => a.Priority == priorities[i]).ToArray();

                    DomainEventPublisher.RaiseMessageEvent(null,
                        $"Running {actionsToRun.Length} priority {priorities[i]} actions.",
                        sourceName);

                    Task t = Parallel.ForEachAsync(actionsToRun,
                        new ParallelOptions()
                        {
                            CancellationToken = cancellationToken,
                            MaxDegreeOfParallelism = maxParallel
                        }, async (action, ct) =>
                            {
                                try
                                {
                                    action.Start();

                                    await dataImportService.ImportDataAsync(action, importConfiguration, ct);

                                    if (action.TargetName.Equals(PurgeName.Imports))
                                    {
                                        // Meta data may have changed as a result of the purge.
                                        var t = dataImportService.ResetMetaDataRepositoryAsync(cancellationToken);
                                        DomainEventPublisher.RaiseMessageEvent(null, $"Resetting meta data.", sourceName);
                                        await t;
                                    }

                                    action.Complete();
                                }
                                catch (Exception exc)
                                {
                                    action.Error(exc);
                                }
                            });

                    // give it a second or two ...
                    await Task.Delay(Convert.ToInt32(TimeSpan.FromSeconds(2).TotalMilliseconds));

                    // wait for tasks to complete in 15 second intervals.
                    while (!t.IsCompleted)
                    {
                        await Task.Delay(Convert.ToInt32(TimeSpan.FromSeconds(15).TotalMilliseconds));
                    }
                }

#if DEBUG
                var endCount = SymbolMetaDataRepository.Count();
                var endLastUpdate = endCount == 0 ? DateTime.MinValue : SymbolMetaDataRepository.GetAll().Select(s => s.LastUpdated).Max();
                var unchanged = SymbolMetaDataRepository.Find(s => s.LastUpdated < startLastUpdate).ToArray();

                Communicate($"start count: {startCount}");
                Communicate($"end count: {endCount}");
                Communicate($"number unchanged: {unchanged.Length}");
#endif
            }

            foreach (var action in cycle.Actions.OrderBy(a => a.UtcCompleted))
            {
                string text = $"{action} {action.Status.GetDescription()} {action.Details}";
                DomainEventPublisher.RaiseMessageEvent(null, text, sourceName);
                if (action.Exception != null)
                {
                    DomainEventPublisher.RaiseMessageEvent(null, action.Exception.ToString(), sourceName);
                }
            }

            var metaTask = dataImportService.SaveMetaDataAsync(cancellationToken);

            DomainEventPublisher.RaiseMessageEvent(null, "Saving symbols to ignore", sourceName);
            await dataImportService.SaveSymbolsToIgnoreAsync(cancellationToken);

            DomainEventPublisher.RaiseMessageEvent(null, "Saving meta data", sourceName);
            await metaTask;
        }

        cycle.Dispose();
    }

    exitCode = 0;
}
catch (Exception exc)
{
    exitCode = 1;
    logger?.LogError(exc, "{MESSAGE}", exc.Message);
    DomainEventPublisher.RaiseMessageEvent(null, exc.ToString(), sourceName, LogLevel.Error);
    Communicate(exc.ToString(), true);
}
finally
{
    if (exitCode == 0)
    {
        if (!dryRun)
        {
            WriteApiUsage();
            DomainEventPublisher.RaiseMessageEvent(null, $"Import completed in {timer.Elapsed:d\\.hh\\:mm\\:ss}.", sourceName, LogLevel.Information);
        }
    }

    timer.Stop();

    if (!showHelp)
    {
        ApiEventPublisher.RaiseMessageEventHandler -= EventPublisher_RaiseMessageEventHandler;
        ApiEventPublisher.RaiseApiLimitReachedEventHandler -= EventPublisher_RaiseApiLimitReachedEventHandler;

        DomainEventPublisher.DatabaseErrorHander -= EventPublisher_DatabaseErrorHander;
        DomainEventPublisher.RaiseMessageEventHandler -= EventPublisher_RaiseMessageEventHandler;
    }

    loggerProvider?.Dispose();
    cts.Dispose();

    Environment.Exit(exitCode);
}

void ShowActionBlocks(ImportCycle cycle)
{
    DataImportService.CalculateCost(cycle, importConfiguration);

    int totalCost = 0;

    foreach (var kvp in cycle.ActionBlocks)
    {
        Communicate($"{Environment.NewLine}Priority {kvp.Key}", true, false);

        foreach (var action in kvp.Value)
        {
            totalCost += action.EstimatedCost ?? 0;
            Communicate($"{action.EstimatedCost.ToString() ?? " ",9}\t{action}", true);
        }
    }

    Communicate($"{Environment.NewLine}{totalCost,9}\tTotal estimated cost.");
}

void EventPublisher_RaiseApiLimitReachedEventHandler(object? sender, ApiLimitReachedEventArgs e)
{
    cts.Cancel();
    DomainEventPublisher.RaiseMessageEvent(null, $"{e.ApiLimitReachedException.Message}", sourceName, LogLevel.Error);
    CommunicateAndLog($"{e.ApiLimitReachedException.Message}");
}

void EventPublisher_RaiseMessageEventHandler(object? sender, MessageEventArgs e)
{
    bool forceMessage = false;

    if (e.Exception != null)
    {
        logger?.LogError(e.Exception, "{MESSAGE}", e.Message);
        forceMessage = true;
    }
    else if (e.LogLevel != LogLevel.None)
    {
        logger?.Log(e.LogLevel, "{MESSAGE}", e.Message);
        forceMessage = e.LogLevel is LogLevel.Critical or LogLevel.Error;
    }

    Communicate(e.Message, forceMessage, true);
}

void EventPublisher_DatabaseErrorHander(object? sender, DatabaseErrorEventArgs e)
{
    DirectoryInfo dbErrorDir = new("errors");
    if (!dbErrorDir.Exists) { dbErrorDir.Create(); }

    var filename = $"{DateTime.Now:yyyyMMddHHmmss}_{e.Source}_{Guid.NewGuid().ToString()[0..3]}.txt";

    var targetFile = new FileInfo(Path.Combine(dbErrorDir.FullName, filename));

    var json = e.Parameters == null ? null : JsonSerializer.Serialize(e.Parameters);

    StringBuilder text = new();
    text.AppendLine($"Exception: {e.Exception}");
    text.AppendLine($"SQL: {e.Sql}");
    text.AppendLine($"Parameters: {json}");

    File.WriteAllText(targetFile.FullName, text.ToString());

    ApiEventPublisher.RaiseMessageEvent(sender, $"File created: {targetFile.FullName}", e.Source, LogLevel.Warning);
}

void WriteApiUsage()
{
    var line = new string('-', 25);
    StringBuilder sb = new(NL);
    sb.AppendLine(line);
    sb.AppendLine($"      Usage: {ApiService.Usage}");
    sb.AppendLine($"Daily Limit: {ApiService.DailyLimit}");
    sb.AppendLine(line);

    Communicate(sb.ToString());
}

void ShowHelp(string? message = null)
{
    if (!string.IsNullOrWhiteSpace(message))
    {
        Console.WriteLine($"{message}{Environment.NewLine}");
    }

    Dictionary<string, string> helpDefinitions = new()
    {
        { "[-k|--key] <API key>","The API key to use. Optional; the key can come from the json configuration or the YAML configuration."},
        { "[-c <yaml config file>]", "Use the specified configuration file. Optional; will default to 'config.yml'" },
        { "[-v|--verbose]", "Write proces details to console." },
        { "[--dry-run]","Estimate the cost of the specified YAML config file."},
        { "[-h|-?|?|--help]", "Show this help." }
    };

    string assemblyName = Assembly.GetExecutingAssembly().GetName().Name ?? sourceName;

    int maxKeyLength = helpDefinitions.Keys.Max(k => k.Length) + 1;

    Console.WriteLine($"{Environment.NewLine}{assemblyName} {string.Join(' ', helpDefinitions.Keys)}{Environment.NewLine}");

    foreach (KeyValuePair<string, string> helpItem in helpDefinitions)
    {
        Console.WriteLine($"{helpItem.Key.PadRight(maxKeyLength)}\t{helpItem.Value}");
    }
}

void HandleArguments(string[] args)
{
    for (int a = 0; a < args.Length; a++)
    {
        string argument = args[a].ToLower();

        switch (argument)
        {
            // This is the first check for the key.
            case "--key":
            case "-k":
                if (a == args.Length - 1) { throw new ArgumentException($"Expected an API key after {args[a]}."); }
                apiKey ??= args[++a];
                break;
            case "-c":
                if (a == args.Length - 1) { throw new ArgumentException($"Expected a path to a YAML file after {args[a]}."); }
                configFileInfo = new FileInfo(args[++a]);
                break;
            case "--dry-run":
                dryRun = true;
                break;
            case "--verbose":
            case "-v":
                verbose = true;
                break;
            case "--help":
            case "-h":
            case "-?":
            case "?":
                showHelp = true;
                break;
            default:
                throw new ArgumentException($"Unknown argument: {args[a]}");
        }
    }
}

Task ConfigureAsync()
{
    IConfigurationBuilder builder = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
        .AddUserSecrets("b0d79919-fd38-486b-b119-b057643058f9");

    configuration = builder.Build();

    // This is the second check for the key.
    apiKey ??= configuration["EodHistoricalDataApiKey"];

    return ConfigureServicesAsync();
}

Task ConfigureServicesAsync()
{
    Communicate("Configuring services.", prefixWithTimestamp: true);

    var services = new ServiceCollection();

    loggerProvider = ServiceFactory.CreateLoggerProvider(configuration);

    if (loggerProvider != null)
    {
        Communicate("Configuring logger.", prefixWithTimestamp: true);
        var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.ClearProviders();
            builder.AddProvider(loggerProvider);
            builder.AddFilter(sourceName, LogLevel.Trace);
            builder.SetMinimumLevel(LogLevel.Trace);
        });

        logger = loggerFactory.CreateLogger<Program>();
        _ = logger.BeginScope(sourceName);
    }

    Communicate("Configuring service factory.", prefixWithTimestamp: true);
    serviceFactory = new ServiceFactory(configuration, logger);

    var metaDataTask = serviceFactory.LoadStaticDataAsync();

    Communicate($"Reading {configFileInfo.FullName}.", prefixWithTimestamp: true);
    importConfiguration = ImportConfiguration.Create(File.ReadAllText(configFileInfo.FullName));
    apiKey ??= importConfiguration.Options.ApiKey; // This is the final check for the key.
    maxParallel = importConfiguration.Options.MaxDegreeOfParallelism ?? maxParallel;

    if (importConfiguration.Options == null)
    {
        importConfiguration.Options = new ConfigurationOptions() { ApiKey = apiKey };
    }
    else
    {
        importConfiguration.Options.ApiKey = apiKey;
    }

    if (importConfiguration.Options.ApiKey == null) { throw new Exception("Could not determine API key."); }

    Communicate("Creating data import service.", prefixWithTimestamp: true);
    dataImportService = serviceFactory.CreateDataImportService(apiKey!);

    Communicate("Finishing load of meta data.", prefixWithTimestamp: true);
    return metaDataTask;
}

void Communicate(string? message, bool force = false, bool prefixWithTimestamp = false)
{
    if (verbose || force)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            Console.WriteLine(message ?? string.Empty);
        }
        else
        {
            string line = prefixWithTimestamp
                ? $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\t{message}"
                : message;
            Console.WriteLine(line);
        }
    }
}

void CommunicateAndLog(string? message, bool force = false, bool prefixWithTimestamp = false)
{
    if (logger != null && !string.IsNullOrWhiteSpace(message))
    {
        logger.LogInformation("{MESSAGE}", message.Trim());
    }

    Communicate(message, force, prefixWithTimestamp);
}