using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;
using NodeConfigurationExtensions = GridDomain.Node.Persistence.Sql.NodeConfigurationExtensions;

namespace GridDomain.Tests.Unit {
    public static class NodeConfigurationDebugExtensions
    {
        public static IActorSystemConfigBuilder ToDebugStandAloneInMemorySystem(this NodeConfiguration conf)
        {
#if DEBUG
            return conf.ToStandAloneInMemorySystem(true);
#else
            return conf.ToStandAloneInMemorySystemConfig(false);
#endif
        }

        public static IActorSystemConfigBuilder ToDebugStandAloneSystem(this IActorSystemConfigBuilder actorSystemConfigBuilder, NodeConfiguration conf, ISqlNodeDbConfiguration persistence)
        {
#if DEBUG
            return actorSystemConfigBuilder.ToStandAloneSystemConfig(persistence, conf.LogLevel, conf.Address, true);
#else
            return actorSystemConfigBuilder.ToStandAloneSystemConfig(persistence, conf.LogLevel, conf.Address, false);
#endif
        }
    }
}