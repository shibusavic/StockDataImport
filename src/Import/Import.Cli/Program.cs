using EodHistoricalData.Sdk;
using Import.AppServices;
using Import.AppServices.Configuration;
using Import.Infrastructure.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using static Import.Infrastructure.Configuration.Constants;

const string DefaultConfigFileName = "test.yml";

ILoggerProvider? loggerProvider = null;
ServiceFactory serviceFactory;
ILogger? logger = null;
IConfiguration configuration;
Stopwatch timer = Stopwatch.StartNew();

string sourceName = "eod-import";
string? apiKey = null;
bool showHelp = false;
bool verbose = false;
FileInfo configFileInfo = new(DefaultConfigFileName);
DataImportService? dataImportService = null;

int exitCode = -1;

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
        Configure();

        Communicate(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        ImportConfiguration importConfiguration = ImportConfiguration.Create(
            File.ReadAllText(configFileInfo.FullName));

        // This is the third check for the key. First one found wins, the selection order is command line first, then JSON config, and finally YAML.
        apiKey ??= importConfiguration.ApiKey;

        if (apiKey == null) { throw new Exception("Could not find API key."); }

        var actionService = serviceFactory.GetImportActionService();

        var actions = await actionService.GetActionItemsAsync(importConfiguration);

        if (actions.Any())
        {
            dataImportService = serviceFactory.GetDataImportService(apiKey, importConfiguration.MaxTokenUsage ?? 100_000);

            dataImportService.ApiResponseExceptionEventHandler += DataImportService_ApiResponseExceptionEventHandler;
            dataImportService.CommunicationEventHandler += DataImportService_CommunicationEventHandler;
            dataImportService.ApiLimitReachedEventHandler += DataImportService_ApiLimitReachedEventHandler;

            WriteUsage();

            foreach (var action in actions)
            {
                Stopwatch actionTimer = Stopwatch.StartNew();

                try
                {
                    Communicate("");

                    if (action.ActionName == ActionNames.Skip)
                    {
                        continue;
                    }

                    if (action.ActionName == ActionNames.Fix)
                    {
                        if (string.IsNullOrWhiteSpace(action.TargetName))
                        {
                            throw new Exception("No target provided for fixes. Check import configuration.");
                        }

                        await dataImportService.ApplyFixAsync(action.TargetName, cancellationToken);
                    }

                    if (action.ActionName == ActionNames.Purge)
                    {
                        if (string.IsNullOrWhiteSpace(action.TargetName))
                        {
                            throw new Exception("No target provided for purge. Check import configuration.");
                        }

                        await dataImportService.PurgeDataAsync(action.TargetName!, cancellationToken);
                    }

                    if (action.ActionName == ActionNames.Import)
                    {
                        await dataImportService.ImportDataAsync(action.TargetScope!, action.TargetName!, action.TargetDataType!, cancellationToken);
                    }

                    if (action.ActionName == ActionNames.Truncate)
                    {
                        if (string.IsNullOrWhiteSpace(action.TargetName))
                        {
                            throw new Exception("No target provided for log truncation. Check import configuration.");
                        }

                        if (DateTime.TryParse(action.TargetScope, out DateTime date))
                        {
                            await dataImportService.TruncateLogsAsync(action.TargetName, date, cancellationToken);
                        }
                        else
                        {
                            throw new Exception($"Could not parse '{action.TargetScope}' as a DateTime.");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    Communicate($"Cancelled {action}");
                }
                catch (Exception exc)
                {
                    if (importConfiguration.CancelOnException.GetValueOrDefault())
                    {
                        cts.Cancel();
                    }
                    Communicate(exc.Message, true);
                }
                finally
                {
                    if (!cts.IsCancellationRequested)
                    {
                        CommunicateFinish(action, ref actionTimer);
                    }
                    if (ApiService.IsLimitReached)
                    {
                        DataImportService_ApiLimitReachedEventHandler(null, new ApiLimitReachedException(ApiService.Usage));
                    }
                    actionTimer.Stop();
                }
            }
        }
    }

    exitCode = 0;
}
catch (Exception exc)
{
    exitCode = 1;
    Communicate(exc.Message, true);
}
finally
{
    if (dataImportService != null)
    {
        dataImportService.ApiResponseExceptionEventHandler -= DataImportService_ApiResponseExceptionEventHandler;
        dataImportService.CommunicationEventHandler -= DataImportService_CommunicationEventHandler;
    }

    WriteUsage();
    timer.Stop();
    Communicate($"Import completed in {timer.Elapsed.ConvertToText()}.");

    loggerProvider?.Dispose();

    Environment.Exit(exitCode);
}

void DataImportService_ApiLimitReachedEventHandler(object? sender, ApiLimitReachedException exc)
{
    cts.Cancel(false);
    Communicate($"{exc.Message}");
}

void CommunicateFinish(ActionItem action, ref Stopwatch timer)
{
    Communicate($"Completed {action} in {timer.Elapsed.ConvertToText()}");
}

void DataImportService_CommunicationEventHandler(object sender, string message)
{
    Communicate(message);
}

void DataImportService_ApiResponseExceptionEventHandler(object sender, ApiResponseException apiResponseException, string[] symbols)
{
    cts.Cancel(true);
    Communicate(apiResponseException.ToString(), true);
    Communicate(string.Join(',', symbols), true);
}

void WriteUsage()
{
    var line = new string('-', 25);
    Communicate("");
    Communicate(line);
    Communicate($"      Usage: {ApiService.Usage}");
    Communicate($"Daily Limit: {ApiService.DailyLimit}");
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
                // This is the first check for the key. First one found wins.
                if (a == args.Length - 1) { throw new ArgumentException($"Expected an API key after {args[a]}."); }
                apiKey ??= args[++a];
                break;
            case "-c":
                if (a == args.Length - 1) { throw new ArgumentException($"Expected a path to a YAML file after {args[a]}."); }
                configFileInfo = new FileInfo(args[++a]);
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
        .AddUserSecrets("260619b6-7b25-459b-8caa-e6d79d0bbb7f");

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

    serviceFactory = new ServiceFactory(configuration, logger);

}

void Communicate(string? message, bool force = false)
{
    if (verbose || force)
    {
        Console.WriteLine(message ?? string.Empty);
    }
}