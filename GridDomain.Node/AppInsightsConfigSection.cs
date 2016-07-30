using System.Configuration;

namespace GridDomain.Node
{
    public class AppInsightsConfigSection : ConfigurationSection, IAppInsightsConfiguration
    {
        public const string SectionName = "appinsights";
        private const string AccessPropertyName = "accessKey";

        public static AppInsightsConfigSection Default
            => (AppInsightsConfigSection) ConfigurationManager.GetSection(SectionName);

        [ConfigurationProperty(AccessPropertyName)]
        public string Key
        {
            get { return (string) this[AccessPropertyName]; }
            set { this[AccessPropertyName] = value; }
        }
    }
}