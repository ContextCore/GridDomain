using Akka.Cluster.Sharding;
using GridDomain.Common;

namespace GridDomain.Node.Cluster.CommandPipe
{


    public sealed class ShardedMessageMetadataExtractor : IMessageExtractor
    {
        public string EntityId(object message) => (message as IShardedMessageMetadataEnvelop)?.EntityId;

        public string ShardId(object message) => (message as IShardedMessageMetadataEnvelop)?.ShardId;

        public object EntityMessage(object message) => (message as IMessageMetadataEnvelop);
    }
}