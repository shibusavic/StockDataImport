using YamlDotNet.Serialization;

namespace Import.AppServices.Configuration;

public class ImportActions
{
    public int Priority { get; set; }

    public bool? Skip { get; set; }

    public string? Scope { get; set; }

    [YamlMember(Alias = "Data Types")]
    public string[]? DataTypes { get; set; }

    public bool IsValidForImport()
    {
        return !Skip.GetValueOrDefault() &&
            Scope != null &&
            (DataTypes?.Any() ?? false);
    }

    public override string ToString()
    {
        if (Skip.GetValueOrDefault()) { return "Skip"; }

        return $"Priority {Priority}, {Scope} actions";
    }
}
