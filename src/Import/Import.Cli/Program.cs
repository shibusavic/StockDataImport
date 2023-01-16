using EodHistoricalData.Sdk;
using EodHistoricalData.Sdk.Events;
using Import.AppServices;
using Import.AppServices.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using System.Text;

const string DefaultConfigFileName = "config.yml";
string NL = Environment.NewLine;

ILoggerProvider? loggerProvider = null;
ServiceFactory serviceFactory;
ILogger? logger = null;
IConfiguration configuration;
Stopwatch timer = Stopwatch.StartNew();

string sourceName = "import";
bool dryRun = false;
string? apiKey = null;
bool showHelp = false;
bool verbose = false;
FileInfo configFileInfo = new(DefaultConfigFileName);
DataImportCycle? dataImportCycle = null;
ImportConfiguration importConfiguration = new();

int exitCode = -1;

HandleArguments(args);

var cts = new CancellationTokenSource();
var cancellationToken = cts.Token;

DomainEventPublisher.RaiseMessageEventHandler += DomainEventPublisher_RaiseMessageEventHandler;
DomainEventPublisher.RaiseApiResponseEventHandler += DomainEventPublisher_RaiseApiResponseEventHandler;
DomainEventPublisher.RaiseApiLimitReachedEventHandler += DomainEventPublisher_RaiseApiLimitReachedEventHandler;

try
{
    if (showHelp)
    {
        ShowHelp();
    }
    else
    {
        Configure();

        if (dryRun)
        {
            Communicate($"{NL}Dry Run (api credit cost estimation){NL}");
        }

        await dataImportCycle!.ExecuteAsync(importConfiguration, dryRun, cancellationToken);

        if (dryRun)
        {
            Communicate("Note that the following costs are estimates", true);
            Communicate("based on the information already present in your database.", true);
            Communicate("If your database is empty or not fully hydrated (especially the symbols table)", true);
            Communicate("the cost estimates below are meaningless.", true);
            Communicate("", true);

            foreach (var action in dataImportCycle.Actions)
            {
                Communicate(action.ToString(), true);
            }

            Communicate($"{NL}Total estimated cost:\t{dataImportCycle.Actions.Select(a => a.EstimatedCost).Sum()}{NL}", true);
        }
    }

    exitCode = 0;
}
catch (Exception exc)
{
    exitCode = 1;
    CommunicateAndLog(exc.ToString(), true);
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

    DomainEventPublisher.RaiseMessageEventHandler -= DomainEventPublisher_RaiseMessageEventHandler;
    DomainEventPublisher.RaiseApiResponseEventHandler -= DomainEventPublisher_RaiseApiResponseEventHandler;
    DomainEventPublisher.RaiseApiLimitReachedEventHandler -= DomainEventPublisher_RaiseApiLimitReachedEventHandler;

    loggerProvider?.Dispose();

    Environment.Exit(exitCode);
}

void DomainEventPublisher_RaiseApiLimitReachedEventHandler(object? sender, ApiLimitReachedEventArgs e)
{
    cts.Cancel();
    CommunicateAndLog($"{e.ApiLimitReachedException.Message}");
}

void DomainEventPublisher_RaiseApiResponseEventHandler(object? sender, ApiResponseEventArgs e)
{
    // TODO: fix
    //dataImportService?.SaveApiResponse(e.Request, e.Response ?? "", e.StatusCode).GetAwaiter().GetResult();
}

void DomainEventPublisher_RaiseMessageEventHandler(object? sender, MessageEventArgs e)
{
    CommunicateAndLog(e.Message);
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
            case "--key":
            case "-k":
                // This is the first check for the key.
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

    // This is the second check for the key. First one found wins.
    apiKey ??= configuration["EodHistoricalDataApiKey"];

    ConfigureServices();
}

void ConfigureServices()
{
    var services = new ServiceCollection();

    loggerProvider = ServiceFactory.CreateLoggerProvider(configuration);

    if (loggerProvider != null)
    {
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

    importConfiguration = ImportConfiguration.Create(File.ReadAllText(configFileInfo.FullName));
    apiKey ??= importConfiguration.ApiKey; // This is the final check for the key.
    importConfiguration.ApiKey = apiKey;

    if (importConfiguration.ApiKey == null) { throw new Exception("Could not determine API key."); }

    serviceFactory = new ServiceFactory(configuration, logger);

    dataImportCycle = serviceFactory.CreateDataImportCycle(importConfiguration);
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