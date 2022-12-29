using System.ComponentModel;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Import.AppServices"),
    InternalsVisibleTo("Import.Infrastructure.Tests"),
    InternalsVisibleTo("Import.AppServices.Tests"),
    InternalsVisibleTo("DynamicProxyGenAssembly2")] // to satisfy Moq
namespace Import.Infrastructure.Abstractions;

/// <summary>
/// Represents the database instances available.
/// </summary>
public enum Database
{
    None = 0,
    Logs,
    Imports
}

[Flags]
public enum ImportActionStatus
{
    None = 0,
    [Description("Not Started")]
    NotStarted = 1 << 0,
    [Description("In Progress")]
    InProgress = 1 << 1,
    Error = 1 << 2,
    [Description("Usage Requirement Met")]
    UsageRequirementMet = 1 << 3,
    Completed = 1 << 4
}