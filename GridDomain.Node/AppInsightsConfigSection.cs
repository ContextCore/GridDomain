using System.Configuration;

namespace GridDomain.Node
{
    public class AppInsightsConfigSection : ConfigurationSection, IAppInsightsConfiguration
    {
        public const string SectionName = "appinsights";
        private const string AccessPropertyName = "accessKey";
        private const string EnabledPropertyName = "enabled";

        public static IAppInsightsConfiguration Default
            =>  ConfigurationManager.GetSection(SectionName) as AppInsightsConfigSection;

        [ConfigurationProperty(AccessPropertyName)]
        public string Key
        {
            get { return (string) this[AccessPropertyName]; }
            set { this[AccessPropertyName] = value; }
        }

        [ConfigurationProperty(EnabledPropertyName)]
        public bool IsEnabled
        {
            get { return (bool)this[EnabledPropertyName]; }
            set { this[EnabledPropertyName] = value; }
        }
    }
}