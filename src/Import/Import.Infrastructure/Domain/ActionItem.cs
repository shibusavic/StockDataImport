using Import.Infrastructure.Abstractions;
using Import.Infrastructure.Configuration;

namespace Import.Infrastructure.Domain;

public class ActionItem : IEquatable<ActionItem?>
{
    public ActionItem(string actionName, string targetName) : this(actionName, targetName, null, null, 0) { }

    public ActionItem(string actionName, string targetName, string? targetScope, string? targetDataType, int priority = 1)
        : this(
        Guid.NewGuid(), priority, actionName, targetName,
        ImportActionStatus.NotStarted, DateTime.UtcNow, null, null, targetScope, targetDataType, null)
    {
    }

    internal ActionItem(Guid globalId,
        int priority,
        string actionName,
        string targetName,
        ImportActionStatus status,
        DateTime utcCreated,
        DateTime? utcStarted = null,
        DateTime? utcCompleted = null,
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

    public string TargetName { get; }

    public string? TargetScope { get; }

    internal int? TargetScopeValue =>
        TargetScope switch
        {
            Constants.DataTypeScopes.Full => 0,
            Constants.DataTypeScopes.Bulk => 1,
            _ => null
        };

    public string? TargetDataType { get; }

    /// <summary>
    /// This is the order in which the actions will be sorted.
    /// </summary>
    internal int? TargetDataTypeSortValue => TargetDataType switch
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

    public int EstimatedCost { get; internal set; }

    internal Exception? Exception { get; set; }

    public override string ToString()
    {
        string targetScope = TargetScope ?? "";
        if (DateTime.TryParse(targetScope, out DateTime dt))
        {
            targetScope = dt.ToString("yyyy-MM-dd");
        }

        List<string> elements = new()
        {
            $"Cost {EstimatedCost}".PadRight(12),
            $"Pr {Priority.ToString().PadLeft(3,'0')}",
            ActionName.PadRight(8),
            targetScope.PadRight(10), 
            TargetName.PadRight(11),
            TargetDataType ?? ""
        };

        elements.RemoveAll(string.IsNullOrWhiteSpace);

        return string.Join(' ', elements).Trim();
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