using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "logs", Schema = "public")]
internal class Log
{
    public Log(
        Guid globalId,
        DateTime utcTimestamp,
        string logLevel,
        string? message,
        string? exception,
        string? logScope,
        int? eventId,
        string? eventName)
    {
        GlobalId = globalId;
        UtcTimestamp = utcTimestamp;
        LogLevel = logLevel;
        Message = message;
        Exception = exception;
        LogScope = logScope;
        EventId = eventId;
        EventName = eventName;
    }


    [ColumnWithKey("global_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid GlobalId { get; }

    [ColumnWithKey("utc_timestamp", Order = 2, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }

    [ColumnWithKey("log_level", Order = 3, TypeName = "text", IsPartOfKey = false)]
    public string LogLevel { get; }

    [ColumnWithKey("message", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string? Message { get; }

    [ColumnWithKey("exception", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string? Exception { get; }

    [ColumnWithKey("log_scope", Order = 6, TypeName = "text", IsPartOfKey = false)]
    public string? LogScope { get; }

    [ColumnWithKey("event_id", Order = 7, TypeName = "integer", IsPartOfKey = false)]
    public int? EventId { get; }

    [ColumnWithKey("event_name", Order = 8, TypeName = "text", IsPartOfKey = false)]
    public string? EventName { get; }
}
