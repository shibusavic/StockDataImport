using Import.Infrastructure.Configuration;
using YamlDotNet.Serialization;

namespace Import.AppServices.Configuration;

public class ImportActions
{
    public ImportActions()
    {
        Mode = Constants.Modes.Economy;
    }

    [YamlMember(Alias = Constants.ConfigurationKeys.Skip)]
    public bool? Skip { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.Priority)]
    public int Priority { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.Mode)]
    public string? Mode { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.Scope)]
    public string? Scope { get; set; }

    [YamlMember(Alias = "Data Types")]
    public string[]? DataTypes { get; set; }

    [YamlIgnore()]
    public bool IsValidForImport => !Skip.GetValueOrDefault() &&
            Scope != null &&
            (DataTypes?.Any() ?? false);

    public override string ToString()
    {
        if (Skip.GetValueOrDefault()) { return "Skip"; }

        return $"Priority {Priority}, {Scope} actions";
    }
}
