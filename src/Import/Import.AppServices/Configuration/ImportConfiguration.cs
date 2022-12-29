using Import.Infrastructure.Configuration;
using YamlDotNet.Serialization;

namespace Import.AppServices.Configuration;

public class ImportConfiguration
{
    public ImportConfiguration()
    {

    }

    private ImportConfiguration(Exception exception)
    {
        Exception = exception;
    }

    [YamlIgnore]
    public Exception? Exception { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.ApiKey)]
    public string? ApiKey { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.MaxTokenUsage)]
    public int? MaxTokenUsage { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.CancelOnException)]
    public bool? CancelOnException { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.Fixes)]
    public string[]? Fixes { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.DatabasePurge)]
    public string[]? Purges { get; set; }

    [YamlMember(Alias = Constants.ConfigurationKeys.LogRetention)]
    public IDictionary<string, string>? LogRetention { get; set; }

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
