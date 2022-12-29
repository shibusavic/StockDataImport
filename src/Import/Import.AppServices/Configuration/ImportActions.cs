using YamlDotNet.Serialization;

namespace Import.AppServices.Configuration;

public class ImportActions
{
    public int Priority { get; set; }

    public bool? Skip { get; set; }

    public string? Scope { get; set; }

    public string[]? Exchanges { get; set; }

    [YamlMember(Alias = "Data Types")]
    public string[]? DataTypes { get; set; }

    public bool IsValidForImport()
    {
        return !Skip.GetValueOrDefault() &&
            Scope != null &&
            (Exchanges?.Any() ?? false) &&
            (DataTypes?.Any() ?? false);
    }

    public override string ToString()
    {
        if (Skip.GetValueOrDefault()) { return "Skip"; }

        int numExchanges = Exchanges?.Length ?? 0;
        int numberDataTypes = DataTypes?.Length ?? 0;
        int totalActions = numExchanges * numberDataTypes;

        if (totalActions == 0) { return "No Action"; }

        return $"Priority {Priority}, {totalActions} {Scope} actions";
    }
}
