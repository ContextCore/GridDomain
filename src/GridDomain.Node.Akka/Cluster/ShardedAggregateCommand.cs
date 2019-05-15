using GridDomain.Aggregates;
using GridDomain.Node.Akka.Actors.Aggregates;

namespace GridDomain.Node.Akka.Cluster
{
    public class ShardedAggregateCommand : IShardEnvelop, IHaveMetadata
    {
        public ShardedAggregateCommand(object message, string entityId, string shardId, string region,
            IMessageMetadata metadata)
        {
            Message = message;
            EntityId = entityId;
            ShardId = shardId;
            Region = region;
            Metadata = metadata;
        }

        public static ShardedAggregateCommand New(ICommand message, bool waitForConfirmation = false, IMessageMetadata metadata = null)
        {
            return new ShardedAggregateCommand(new AggregateActor.ExecuteCommand(message, metadata, waitForConfirmation), message.Recipient.ToString(),
                ShardIdGenerator.Instance.GetShardId(message.Recipient.Id),
                message.Recipient.Name,
                metadata ?? MessageMetadata.Empty);
        }

        public IMessageMetadata Metadata { get; private set; }
        public string EntityId { get; private set; }
        public string ShardId { get; private set; }
        public string Region { get; private set; }
        public object Message { get; private set; }
    }
}