using Import.Infrastructure.Abstractions;
using Import.Infrastructure.Configuration;

namespace Import.Infrastructure.Domain;

public class ActionItem : IEquatable<ActionItem?>
{
    public ActionItem(string actionName, string targetName) : this(actionName, targetName, null, null, 0) { }

    public ActionItem(string actionName, string? targetName, string? targetScope, string? targetDataType, int priority = 1) 
        : this(
        Guid.NewGuid(), priority, actionName, ImportActionStatus.NotStarted, DateTime.UtcNow, null, null, targetName, targetScope, targetDataType, null)
    {

    }

    internal ActionItem(Guid globalId,
        int priority,
        string actionName,
        ImportActionStatus status,
        DateTime utcCreated,
        DateTime? utcStarted = null,
        DateTime? utcCompleted = null,
        string? targetName = null,
        string? targetScope = null,
        string? targetDataType = null,
        string? details = null)
    {
        GlobalId = globalId;
        ActionName = actionName;
        TargetName = targetName;
        TargetScope = targetScope;
        TargetDataType = targetDataType;
        Priority = priority;
        UtcCreated = utcCreated;
        UtcStarted = utcStarted;
        UtcCompleted = utcCompleted;
        Status = status;
        Details = details;
    }

    public Guid GlobalId { get; }

    public string ActionName { get; }

    public string? TargetName { get; }

    public string? TargetScope { get; }

    internal int? TargetScopeValue =>
        TargetScope switch
        {
            Constants.DataTypeScopes.Full => 0,
            Constants.DataTypeScopes.Bulk => 1,
            _ => null
        };

    public string? TargetDataType { get; }

    internal int? TargetDataTypeValue => TargetDataType switch
    {
        Constants.DataTypes.Symbols => 0,
        Constants.DataTypes.Prices => 1,
        Constants.DataTypes.Options => 2,
        Constants.DataTypes.Dividends => 3,
        Constants.DataTypes.Splits => 4,
        Constants.DataTypes.Fundamentals => 5,
        _ => null
    };

    public int Priority { get; }

    public ImportActionStatus Status { get; internal set; }

    public DateTime UtcCreated { get; }

    public DateTime? UtcStarted { get; internal set; }

    public DateTime? UtcCompleted { get; internal set; }

    public string? Details { get; internal set; }

    internal Exception? Exception { get; set; }

    public override string ToString()
    {
        List<string> elements = new() { $"Priority {Priority}", ActionName, TargetScope ?? "", TargetName ?? "", TargetDataType ?? "" };

        elements.RemoveAll(e => string.IsNullOrWhiteSpace(e));

        return string.Join(' ', elements);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ActionItem);
    }

    public bool Equals(ActionItem? other)
    {
        return other is not null &&
               GlobalId.Equals(other.GlobalId);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GlobalId);
    }

    public void Start()
    {
        UtcStarted = DateTime.UtcNow;
        Status = ImportActionStatus.InProgress;
        Exception = null;
    }

    public void Complete()
    {
        UtcCompleted = DateTime.UtcNow;
        Status = ImportActionStatus.Completed;
        Exception = null;
    }

    public void Error(Exception? exception = null)
    {
        UtcCompleted = DateTime.UtcNow;
        Status = ImportActionStatus.Error;
        Exception = exception;
    }

    public void Quit(ImportActionStatus status = ImportActionStatus.UsageRequirementMet)
    {
        UtcCompleted = DateTime.UtcNow;
        Status = status;
        Exception = null;
    }
}