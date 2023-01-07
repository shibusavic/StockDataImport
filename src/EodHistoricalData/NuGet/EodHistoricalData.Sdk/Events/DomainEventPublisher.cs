namespace EodHistoricalData.Sdk.Events;

public static class DomainEventPublisher
{
    public static event EventHandler<MessageEventArgs>? RaiseMessageEventHandler;
    public static event EventHandler<ApiLimitReachedEventArgs>? RaiseApiLimitReachedEventHandler;
    public static event EventHandler<ApiResponseEventArgs>? RaiseApiResponseEventHandler;

    public static void RaiseMessageEvent(object? sender, string message, string source)
    {
        if (!string.IsNullOrWhiteSpace(message))
        {
            RaiseMessageEventHandler?.Invoke(sender, new MessageEventArgs(message, source));
        }
    }

    public static void RaiseMessageEvent(object? sender, string context, string message, string source)
    {
        if (!string.IsNullOrWhiteSpace(context) && !string.IsNullOrWhiteSpace(message))
        {
            RaiseMessageEventHandler?.Invoke(sender, new MessageEventArgs(context, message, source));
        }
    }

    public static void RaiseApiLimitReachedEvent(object? sender, ApiLimitReachedException exception, string source)
    {
        RaiseApiLimitReachedEventHandler?.Invoke(sender, new ApiLimitReachedEventArgs(exception, source));
    }

    public static void RaiseApiResponseEvent(object? sender, int statusCode, string request, string response, string source)
    {
        RaiseApiResponseEventHandler?.Invoke(sender, new ApiResponseEventArgs(statusCode, request, response, source));
    }

    public static void RaiseApiResponseEvent(object? sender, int statusCode, string request,
        ApiResponseException apiResponseException, string source)
    {
        RaiseApiResponseEventHandler?.Invoke(sender, new ApiResponseEventArgs(statusCode, request, apiResponseException, source));
    }
}