using EodHistoricalData.Sdk;
using EodHistoricalData.Sdk.Events;
using Import.AppServices;
using Import.AppServices.Configuration;
using Import.Infrastructure.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;

Stopwatch timer = Stopwatch.StartNew();

string NL = Environment.NewLine;
int exitCode = -1;

const string DefaultConfigFileName = "config.yml";
const string sourceName = "import";

ILoggerProvider? loggerProvider = null;
ServiceFactory serviceFactory;
ILogger? logger = null;
IConfiguration configuration;

DataImportService? dataImportService = null;
DataImportCycle? dataImportCycle = null;
ImportConfiguration importConfiguration = new();
FileInfo configFileInfo = new(DefaultConfigFileName);

bool dryRun = false;
string? apiKey = null;
bool showHelp = false;
bool verbose = false;

HandleArguments(args);

var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;

if (!showHelp)
{

    ApiEventPublisher.RaiseMessageEventHandler += EventPublisher_RaiseMessageEventHandler;
    ApiEventPublisher.RaiseApiLimitReachedEventHandler += EventPublisher_RaiseApiLimitReachedEventHandler;

    DomainEventPublisher.DatabaseErrorHander += EventPublisher_DatabaseErrorHander;
    DomainEventPublisher.RaiseMessageEventHandler += EventPublisher_RaiseMessageEventHandler;
}

try
{
    if (showHelp)
    {
        ShowHelp();
    }
    else
    {
        Configure();

        string welcomeMsg = dryRun ? "Dry Run (api credit cost estimation)" : "Welcome. Configuration complete.";
        Communicate(welcomeMsg, false, true);

        await dataImportCycle!.ExecuteAsync(importConfiguration, dryRun, cancellationToken);

        if (dryRun)
        {
            Communicate("Note that the following costs are estimates based on the information already present in your database.", true);
            Communicate("If the database is empty or the symbols table is empty, the estimates will be zero.", true);
            Communicate("Attempts are made along the way to prevent exceeding the specified API call limit.", true);
            Communicate("", true);

            foreach (var action in dataImportCycle.Actions)
            {
                Communicate(action.ToString(), true);
            }

            Communicate($"{NL}Total estimated cost:\t{dataImportCycle.Actions.Select(a => a.EstimatedCost).Sum()}{NL}", true);
        }
        else
        {
            if (dataImportService != null)
            {
                Communicate("Saving symbols to ignore", false, true);
                await dataImportService.SaveSymbolsToIgnoreAsync(cancellationToken);
            }
        }
    }

    exitCode = 0;
}
catch (Exception exc)
{
    exitCode = 1;
    logger?.LogError(exc, "{MESSAGE}", exc.Message);
    Communicate(exc.ToString(), true);
}
finally
{
    if (exitCode == 0)
    {
        if (!dryRun)
        {
            WriteApiUsage();
            CommunicateAndLog($"Import completed in {timer.Elapsed.ConvertToText()}.");
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

    dataImportCycle?.Dispose();
    loggerProvider?.Dispose();
    cts.Dispose();

    Environment.Exit(exitCode);
}

void EventPublisher_RaiseApiLimitReachedEventHandler(object? sender, ApiLimitReachedEventArgs e)
{
    cts.Cancel();
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

    Communicate(line);
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

void Configure()
{
    IConfigurationBuilder builder = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
        .AddUserSecrets("b0d79919-fd38-486b-b119-b057643058f9");

    configuration = builder.Build();

    // This is the second check for the key.
    apiKey ??= configuration["EodHistoricalDataApiKey"];

    ConfigureServices();
}

void ConfigureServices()
{
    Communicate("Configuring services ...", prefixWithTimestamp: true);

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

    Communicate($"Reading {configFileInfo.FullName}", prefixWithTimestamp: true);
    importConfiguration = ImportConfiguration.Create(File.ReadAllText(configFileInfo.FullName));
    apiKey ??= importConfiguration.ApiKey; // This is the final check for the key.
    importConfiguration.ApiKey = apiKey;

    if (importConfiguration.ApiKey == null) { throw new Exception("Could not determine API key."); }

    serviceFactory = new ServiceFactory(configuration, logger);

    Communicate("Creating data import cycle; this might take a few minutes.", prefixWithTimestamp: true);
    dataImportCycle = serviceFactory.CreateDataImportCycle(importConfiguration);
    dataImportService = serviceFactory.CreateDataImportService(apiKey!);
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
            Console.WriteLine(prefixWithTimestamp
                ? $"{DateTime.Now:yyyy-MM-dd HH:mm}\t{message}"
                : message);
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