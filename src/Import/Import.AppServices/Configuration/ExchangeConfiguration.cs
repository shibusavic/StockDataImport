using YamlDotNet.Serialization;

namespace Import.AppServices.Configuration
{
    public class ExchangeConfiguration
    {
        public ExchangeConfiguration(
            Dictionary<string, string[]>? filters = null)
        {
            Filters = filters;
        }

        [YamlMember(Alias = "Filters")]
        public Dictionary<string, string[]>? Filters { get; set; }
    }
}
