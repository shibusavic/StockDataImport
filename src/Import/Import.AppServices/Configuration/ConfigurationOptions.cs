using Import.Infrastructure.Configuration;
using YamlDotNet.Serialization;

namespace Import.AppServices.Configuration
{
    public class ConfigurationOptions
    {
        [YamlMember(Alias = Constants.ConfigurationKeys.ApiKey)]
        public string? ApiKey { get; set; }

        [YamlMember(Alias = Constants.ConfigurationKeys.CancelOnException)]
        public bool? CancelOnException { get; set; }

        [YamlMember(Alias = Constants.ConfigurationKeys.ReasonsToIgnore)]
        public string[]? ReasonsToIgnore { get; set; }

        [YamlMember(Alias = Constants.ConfigurationKeys.DataRetention)]
        public IDictionary<string, string>? DataRetention { get; set; }

        [YamlMember(Alias = Constants.ConfigurationKeys.MaxDegreeOfParallelism)]
        public int? MaxDegreeOfParallelism { get; set; }
    }
}
