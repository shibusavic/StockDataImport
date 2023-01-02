using System.ComponentModel.DataAnnotations.Schema;
using Shibusa.Data;
using Shibusa.Extensions;

namespace Import.Infrastructure.PostgreSQL.DataAccessObjects;

[Table(name: "action_items", Schema = "public")]
internal class ActionLog
{
    public ActionLog(
        Guid globalId,
        string actionName,
        string? targetName,
        string? targetScope,
        string? targetDataType,
        int priority,
        string status,
        DateTime utcCreated,
        DateTime? utcStarted,
        DateTime? utcCompleted,
        string? details,
        DateTime utcTimestamp)
    {
        GlobalId = globalId;
        ActionName = actionName;
        TargetName = targetName;
        TargetScope = targetScope;
        TargetDataType = targetDataType;
        Priority = priority;
        Status = status;
        UtcCreated = utcCreated;
        UtcStarted = utcStarted;
        UtcCompleted = utcCompleted;
        Details = details;
        UtcTimestamp = utcTimestamp;
    }

    public ActionLog(Domain.ActionItem action)
    {
        GlobalId = action.GlobalId;
        ActionName = action.ActionName;
        TargetName = action.TargetName;
        TargetScope = action.TargetScope;
        TargetDataType = action.TargetDataType;
        Priority = action.Priority;
        Status = action.Status.GetDescription();
        UtcCreated = action.UtcCreated;
        UtcStarted = action.UtcStarted;
        UtcCompleted = action.UtcCompleted;
        Details = action.Details ?? action.Exception?.Message;
        UtcTimestamp = DateTime.UtcNow;
    }

    [ColumnWithKey("global_id", Order = 1, TypeName = "uuid", IsPartOfKey = true)]
    public Guid GlobalId { get; }

    [ColumnWithKey("action_name", Order = 2, TypeName = "text", IsPartOfKey = false)]
    public string ActionName { get; }

    [ColumnWithKey("target_name", Order = 3, TypeName = "text", IsPartOfKey = false)]
    public string? TargetName { get; }

    [ColumnWithKey("target_scope", Order = 4, TypeName = "text", IsPartOfKey = false)]
    public string? TargetScope { get; }

    [ColumnWithKey("target_data_type", Order = 5, TypeName = "text", IsPartOfKey = false)]
    public string? TargetDataType { get; }

    [ColumnWithKey("priority", Order = 6, TypeName = "integer", IsPartOfKey = false)]
    public int Priority { get; }

    [ColumnWithKey("status", Order = 7, TypeName = "text", IsPartOfKey = false)]
    public string Status { get; }

    [ColumnWithKey("utc_created", Order = 8, TypeName = "timestamp without time zone", IsPartOfKey = false)]
    public DateTime UtcCreated { get; }

    [ColumnWithKey("utc_started", Order = 9, TypeName = "timestamp without time zone", IsPartOfKey = false)]
    public DateTime? UtcStarted { get; }

    [ColumnWithKey("utc_completed", Order = 10, TypeName = "timestamp without time zone", IsPartOfKey = false)]
    public DateTime? UtcCompleted { get; }

    [ColumnWithKey("details", Order = 11, TypeName = "text", IsPartOfKey = false)]
    public string? Details { get; }

    [ColumnWithKey("utc_timestamp", Order = 12, TypeName = "timestamp with time zone", IsPartOfKey = false)]
    public DateTime UtcTimestamp { get; }

    public Domain.ActionItem? ToDomainObject()
    {
        if (GlobalId == Guid.Empty) { return null; }

        var status = EnumExtensions.GetEnum<Abstractions.ImportActionStatus>(Status);

        return new Domain.ActionItem(
            GlobalId,
            Priority,
            ActionName,
            status,
            UtcCreated,
            UtcStarted,
            UtcCompleted,
            TargetName,
            TargetScope,
            TargetDataType,
            Details);
    }
}
