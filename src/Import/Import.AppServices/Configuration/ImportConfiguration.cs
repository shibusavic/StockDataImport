using Import.Infrastructure.Configuration;
using YamlDotNet.Serialization;

namespace Import.AppServices.Configuration;

public class ImportConfiguration
{
    public static class ReasonToIgnoreValues
    {
        public const string MissingFundamentals = "No Fundamentals";
    }

    public ImportConfiguration()
    {
        GlobalId = Guid.NewGuid();
        Options = new ConfigurationOptions();
    }

    private ImportConfiguration(Exception exception) : this()
    {
        Exception = exception;
    }

    [YamlIgnore]
    public Guid GlobalId { get; }

    [YamlIgnore]
    public Exception? Exception { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.Options)]
    public ConfigurationOptions Options { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.Exchanges)]
    public IDictionary<string, IDictionary<string, string[]>>? Exchanges { get; set; }

    public string[] GetExchanges() =>
        (Exchanges?.Any() ?? false) ? Exchanges.Select(e => e.Key).ToArray() : Array.Empty<string>();

    public string[] GetSubExchanges(string exchangeCode) =>
        (Exchanges?.Any() ?? false) && Exchanges.ContainsKey(exchangeCode) &&
            Exchanges[exchangeCode].ContainsKey("Exchanges")
        ? Exchanges[exchangeCode]["Exchanges"]
        : Array.Empty<string>();

    [YamlMember(Alias = Constants.ConfigurationKeys.DatabasePurge)]
    public string[]? Purges { get; set; }

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
}
