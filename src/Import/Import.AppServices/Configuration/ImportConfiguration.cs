using Import.Infrastructure.Configuration;
using YamlDotNet.Serialization;

namespace Import.AppServices.Configuration;

public class ImportConfiguration : IEquatable<ImportConfiguration?>
{
    public static class ReasonToIgnoreValues
    {
        public const string MissingFundamentals = "No Fundamentals";
    }

    public ImportConfiguration()
    {
        GlobalId = Guid.NewGuid();
    }

    private ImportConfiguration(Exception exception) : this()
    {
        Exception = exception;
    }

    [YamlIgnore]
    public Guid GlobalId { get; }

    [YamlIgnore]
    public Exception? Exception { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.ApiKey)]
    public string? ApiKey { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.CancelOnException)]
    public bool? CancelOnException { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.Exchanges)]
    public Dictionary<string, Dictionary<string, string[]>>? Exchanges { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.ReasonsToIgnore)]
    public string[]? ReasonsToIgnore { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.DatabasePurge)]
    public string[]? Purges { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.DataRetention)]
    public IDictionary<string, string>? DataRetention { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.OnEmptyDb)]
    public ImportActions[]? OnEmptyDatabase { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.AnyDay)]
    public ImportActions[]? AnyDay { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.Sunday)]
    public ImportActions[]? Sunday { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.Monday)]
    public ImportActions[]? Monday { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.Tuesday)]
    public ImportActions[]? Tuesday { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.Wednesday)]
    public ImportActions[]? Wednesday { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.Thursday)]
    public ImportActions[]? Thursday { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.Friday)]
    public ImportActions[]? Friday { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.Saturday)]
    public ImportActions[]? Saturday { get; set; }

    //public int GetExchangeCount()
    //{
    //    List<string> exchanges = new();

    //    exchanges.AddRange(ExtractExchanges(OnEmptyDatabase));
    //    exchanges.AddRange(ExtractExchanges(AnyDay));
    //    exchanges.AddRange(ExtractExchanges(Sunday));
    //    exchanges.AddRange(ExtractExchanges(Monday));
    //    exchanges.AddRange(ExtractExchanges(Tuesday));
    //    exchanges.AddRange(ExtractExchanges(Wednesday));
    //    exchanges.AddRange(ExtractExchanges(Thursday));
    //    exchanges.AddRange(ExtractExchanges(Friday));
    //    exchanges.AddRange(ExtractExchanges(Saturday));

    //    return exchanges.Distinct().Count();
    //}

    //private IEnumerable<string> ExtractExchanges(ImportActions[]? actions)
    //{
    //    if (actions?.Any() ?? false)
    //    {
    //        foreach (var exchange in actions.SelectMany(x => x.Exchanges ?? Array.Empty<string>()).Distinct())
    //        {
    //            yield return exchange;
    //        }
    //    }
    //}

    public static ImportConfiguration Create(string yaml)
    {
        if (string.IsNullOrWhiteSpace(yaml)) { throw new ArgumentNullException(nameof(yaml)); }

        try
        {
            var deserializer = new DeserializerBuilder().Build();
            return deserializer.Deserialize<ImportConfiguration>(yaml);
        }
        catch (Exception exc)
        {
            return new ImportConfiguration(exc);
        }
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ImportConfiguration);
    }

    public bool Equals(ImportConfiguration? other)
    {
        return other is not null &&
               GlobalId.Equals(other.GlobalId);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(GlobalId);
    }

    public static bool operator ==(ImportConfiguration? left, ImportConfiguration? right)
    {
        return EqualityComparer<ImportConfiguration>.Default.Equals(left, right);
    }

    public static bool operator !=(ImportConfiguration? left, ImportConfiguration? right)
    {
        return !(left == right);
    }
}
