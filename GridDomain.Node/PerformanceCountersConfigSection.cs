using System.Configuration;

namespace GridDomain.Node
{
    public class PerformanceCountersConfigSection : ConfigurationSection, IPerformanceCountersConfiguration
    {
        public const string SectionName = "perfcounters";
        private const string EnabledPropertyName = "enabled";

        public static IPerformanceCountersConfiguration Default
            => ConfigurationManager.GetSection(SectionName) as PerformanceCountersConfigSection;

        [ConfigurationProperty(EnabledPropertyName)]
        public bool IsEnabled
        {
            get { return (bool)this[EnabledPropertyName]; }
            set { this[EnabledPropertyName] = value; }
        }
    }
}