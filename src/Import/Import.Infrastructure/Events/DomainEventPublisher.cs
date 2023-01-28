using EodHistoricalData.Sdk.Events;
using Microsoft.Extensions.Logging;

namespace Import.Infrastructure.Events
{
    public static class DomainEventPublisher
    {
        public static event EventHandler<DatabaseErrorEventArgs>? DatabaseErrorHander;
        public static event EventHandler<MessageEventArgs>? RaiseMessageEventHandler;

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

        public static void RaiseDatabaseError(object? sender, string source, Exception exception,
            string? sql = null, object? parameters = null)
        {
            DatabaseErrorHander?.Invoke(sender, new DatabaseErrorEventArgs(source, exception,
                sql, parameters));
        }
    }
}
