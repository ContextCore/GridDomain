using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging;

namespace GridDomain.Node.Cluster.CommandPipe {
    public class ShardedCommandMetadataEnvelop : IShardedMessageMetadataEnvelop
    {
        private readonly IHaveId _command;
        public object Message => _command;

        public IMessageMetadata Metadata { get; }
        public string EntityId { get; }
        public string ShardId { get; }

        public ShardedCommandMetadataEnvelop(ICommand message, IMessageMetadata metadata = null, IShardIdGenerator generator=null)
        {
            _command = message;
            Metadata = metadata ?? MessageMetadata.Empty;
            generator = generator ?? DefaultShardIdGenerator.Instance;
            ShardId = generator.Resolve(message.AggregateId);
            EntityId = EntityActorName.GetFullName(message.AggregateType, message.AggregateId);
        }
    }
}