using EodHistoricalData.Sdk;
using EodHistoricalData.Sdk.Events;
using Import.AppServices;
using Import.AppServices.Configuration;
using Import.Infrastructure.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using static Import.Infrastructure.Configuration.Constants;

const string DefaultConfigFileName = "config.yml";

ILoggerProvider? loggerProvider = null;
ServiceFactory serviceFactory;
ILogger? logger = null;
IConfiguration configuration;
Stopwatch timer = Stopwatch.StartNew();

string sourceName = "import";
string? apiKey = null;
bool showHelp = false;
bool verbose = false;
FileInfo configFileInfo = new(DefaultConfigFileName);
DataImportService? dataImportService = null;
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
        CommunicateAndLog($"Import started at {DateTime.Now.ToString("yyyy-MM-dd HH:mm")}");

        importConfiguration = ImportConfiguration.Create(File.ReadAllText(configFileInfo.FullName));

        apiKey ??= importConfiguration.ApiKey; // This is the final check for the key.

        Configure();
        Validate(apiKey);

        if (dataImportService == null)
        {
            throw new Exception("Unable to instantiate data import service.");
        }

        var actionService = serviceFactory.GetImportActionService();

        var actions = await actionService.GetActionItemsAsync(importConfiguration);

        if (actions.Any())
        {
            var t = actionService.SaveActionItemsAsync(actions, cancellationToken);

            CommunicateAndLog($"{actions.Count()} action(s) identified.");

            WriteApiUsage();

            await t;

            foreach (var action in actions)
            {
                Stopwatch actionTimer = Stopwatch.StartNew();

                action.Start();
                await actionService.SaveActionItemsAsync(new ActionItem[] { action }, cancellationToken);

                try
                {
                    CommunicateAndLog("");

                    if (action.ActionName == ActionNames.Skip)
                    {
                        action.Complete();
                        await actionService.SaveActionItemsAsync(new ActionItem[] { action }, cancellationToken);
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
                        await dataImportService.ImportDataAsync(action.TargetScope!, action.TargetName!,
                            action.TargetDataType!, cancellationToken);
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
                catch (OperationCanceledException oce)
                {
                    CommunicateAndLog($"Cancelled {action}");

                    action.Error(oce);
                }
                catch (Exception exc)
                {
                    if (importConfiguration.CancelOnException.GetValueOrDefault())
                    {
                        cts.Cancel();
                    }

                    CommunicateAndLog(exc.Message, true);

                    action.Error(exc);
                }
                finally
                {
                    if (!cts.IsCancellationRequested)
                    {
                        CommunicateFinish(action, ref actionTimer);
                    }

                    //if (ApiService.LimitReached)
                    //{
                    //    DataImportService_ApiLimitReachedEventHandler(null, new ApiLimitReachedException(ApiService.Usage));
                    //}

                    if (action.Status == Import.Infrastructure.Abstractions.ImportActionStatus.InProgress)
                    {
                        action.Complete();
                    }

                    actionTimer.Stop();
                }
            }

            t = actionService.SaveActionItemsAsync(actions, CancellationToken.None);

            CommunicateAndLog("Updating action logs.");

            await t;
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
        WriteApiUsage();
        CommunicateAndLog($"Import completed in {timer.Elapsed.ConvertToText()}.");
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
    dataImportService?.SaveApiResponse(e.Request, e.Response ?? "", e.StatusCode).GetAwaiter().GetResult();
}

void DomainEventPublisher_RaiseMessageEventHandler(object? sender, MessageEventArgs e)
{
    CommunicateAndLog(e.Message);
}

void CommunicateFinish(ActionItem action, ref Stopwatch timer)
{
    CommunicateAndLog($"Completed {action} in {timer.Elapsed.ConvertToText()}");
}

void WriteApiUsage()
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
                // This is the first check for the key.
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
        .AddUserSecrets("b0d79919-fd38-486b-b119-b057643058f9");

    configuration = builder.Build();

    // This is the second check for the key. First one found wins.
    apiKey ??= configuration["EodHistoricalDataApiKey"];

    ConfigureServices();
}

void Validate(string? apiKey)
{
    if (apiKey == null) { throw new Exception("Could not find API key."); }

    dataImportService = serviceFactory.GetDataImportService(apiKey, importConfiguration.MaxTokenUsage ?? 100_000);
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
        if (string.IsNullOrWhiteSpace(message))
        {
            Console.WriteLine(message ?? string.Empty);
        }
        else
        {
            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm")}\t{message}");
        }
    }
}
void CommunicateAndLog(string? message, bool force = false)
{
    if (logger != null && !string.IsNullOrWhiteSpace(message))
    {
        logger.LogInformation("{MESSAGE}", message);
    }

    Communicate(message, force);
}