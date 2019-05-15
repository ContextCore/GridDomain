using Akka.Cluster.Sharding;

namespace GridDomain.Node.Akka
{
    public sealed class ShardedMessageMetadataExtractor : IMessageExtractor
    {
        public string EntityId(object message) => (message as IShardEnvelop)?.EntityId;

        public string ShardId(object message) => (message as IShardEnvelop)?.ShardId;

        public object EntityMessage(object message) => (message as IShardEnvelop)?.Message;
    }
}