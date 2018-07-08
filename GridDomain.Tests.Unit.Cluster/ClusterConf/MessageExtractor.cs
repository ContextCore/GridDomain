using Akka.Cluster.Sharding;
using GridDomain.Node.Cluster;
using GridDomain.Node.Cluster.CommandPipe;

namespace GridDomain.Tests.Unit.Cluster.ClusterConf {
    public sealed class MessageExtractor : IMessageExtractor
    {
        public string EntityId(object message) => (message as ShardEnvelope)?.EntityId;

        public string ShardId(object message) => (message as ShardEnvelope)?.ShardId;

        public object EntityMessage(object message) => (message as ShardEnvelope)?.Message;
    }
}