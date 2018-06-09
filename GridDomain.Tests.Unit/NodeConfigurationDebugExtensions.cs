using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;

namespace GridDomain.Tests.Unit {
    public static class NodeConfigurationDebugExtensions
    {
        public static ActorSystemBuilder ToDebugStandAloneInMemorySystem(this NodeConfiguration conf)
        {
#if DEBUG
            return conf.ToStandAloneInMemorySystem(true);
#else
            return conf.ToStandAloneInMemorySystemConfig(false);
#endif
        }

        public static ActorSystemBuilder ToDebugStandAloneSystem(this NodeConfiguration conf, ISqlNodeDbConfiguration persistence)
        {
#if DEBUG
            return conf.ToStandAloneSystemConfig(persistence, true);
#else
            return conf.ToStandAloneSystemConfig(persistence, false);
#endif
        }
    }
}