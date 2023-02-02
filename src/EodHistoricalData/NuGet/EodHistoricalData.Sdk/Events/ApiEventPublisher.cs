using Microsoft.Extensions.Logging;

namespace EodHistoricalData.Sdk.Events;

public static class ApiEventPublisher
{
    public const string MissingFundamentalsMessagePrefix = "No fundamentals found for";

    public static event EventHandler<MessageEventArgs>? RaiseMessageEventHandler;
    public static event EventHandler<ApiLimitReachedEventArgs>? RaiseApiLimitReachedEventHandler;
    public static event EventHandler<ApiResponseEventArgs>? RaiseApiResponseEventHandler;

    public static void RaiseMessageEvent(object? sender, string message, string source,
        LogLevel logLevel = LogLevel.None)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            RaiseMessageEventHandler?.Invoke(sender, new MessageEventArgs(source, source, message, null, logLevel));
        }
    }

    public static void RaiseMessageEvent(object? sender, string context, string message, string source,
        LogLevel logLevel = LogLevel.None)
    {
        if (!string.IsNullOrWhiteSpace(context) && !string.IsNullOrWhiteSpace(message))
        {
            RaiseMessageEventHandler?.Invoke(sender, new MessageEventArgs(source, context, message, null, logLevel));
        }
    }

    public static void RaiseApiLimitReachedEvent(object? sender, ApiLimitReachedException exception, string source)
    {
        RaiseApiLimitReachedEventHandler?.Invoke(sender, new ApiLimitReachedEventArgs(exception, source));
    }

    public static void RaiseApiResponseEvent(object? sender, int statusCode, string request, string response, string source,
        ApiResponseException? apiResponseException = null)
    {
        RaiseApiResponseEventHandler?.Invoke(sender,
            new ApiResponseEventArgs(statusCode, request, response, source, apiResponseException));
    }
}