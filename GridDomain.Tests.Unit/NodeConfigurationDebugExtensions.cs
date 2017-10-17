using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;

namespace GridDomain.Tests.Unit {
    public static class NodeConfigurationDebugExtensions
    {
        public static string ToDebugStandAloneInMemorySystemConfig(this NodeConfiguration conf)
        {
#if DEBUG
            return conf.ToStandAloneInMemorySystemConfig(true);
#else
            return conf.ToStandAloneInMemorySystemConfig(false);
#endif
        }

        public static string ToDebugStandAloneSystemConfig(this NodeConfiguration conf, ISqlNodeDbConfiguration persistence)
        {
#if DEBUG
            return conf.ToStandAloneSystemConfig(persistence, true);
#else
            return conf.ToStandAloneSystemConfig(persistence, false);
#endif
        }
    }
}