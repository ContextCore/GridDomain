using GridDomain.Common;

namespace GridDomain.Node.Cluster.CommandPipe {
    public sealed class ShardEnvelope:IMessageMetadataEnvelop
    {
        public readonly string ShardId;
        public readonly string EntityId;
        public ShardEnvelope(string shardId, string entityId, object message, IMessageMetadata metadata)
        {
            ShardId = shardId;
            EntityId = entityId;
            Message = message;
            Metadata = metadata;
        }

        public object Message { get; }
        public IMessageMetadata Metadata { get; }
    }
}