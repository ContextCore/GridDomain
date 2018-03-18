using Akka.Cluster.Sharding;

namespace GridDomain.Node.Cluster {
    public sealed class MessageExtractor : IMessageExtractor
    {
        public string EntityId(object message) => (message as ShardEnvelope)?.EntityId;

        public string ShardId(object message) => (message as ShardEnvelope)?.ShardId;

        public object EntityMessage(object message) => (message as ShardEnvelope)?.Message;
    }
}