using GridDomain.Node.Configuration;
using GridDomain.Node.Persistence.Sql;
using NodeConfigurationExtensions = GridDomain.Node.Persistence.Sql.NodeConfigurationExtensions;

namespace GridDomain.Tests.Unit {
    public static class NodeConfigurationDebugExtensions
    {
        public static IActorSystemConfigBuilder ToDebugStandAloneInMemorySystem(this NodeConfiguration conf)
        {
            return conf.ToStandAloneInMemorySystem(true);
        }
    }
}