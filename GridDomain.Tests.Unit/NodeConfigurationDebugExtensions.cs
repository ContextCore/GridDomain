using GridDomain.Node.Configuration;

namespace GridDomain.Tests.Unit {
    public static class NodeConfigurationDebugExtensions
    {
        public static string ToDebugStandAloneSystemConfig(this NodeConfiguration conf)
        {
#if DEBUG
            return conf.ToStandAloneSystemConfig(true);
#else
            return conf.ToStandAloneSystemConfig(false);
#endif
        }

        public static string ToDebugStandAloneInMemorySystemConfig(this NodeConfiguration conf)
        {
#if DEBUG
            return conf.ToStandAloneInMemorySystemConfig(true);
#else
            return conf.ToStandAloneInMemorySystemConfig(false);
#endif
        }
    }
}