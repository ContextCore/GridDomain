using System;
using System.Security.Cryptography.X509Certificates;
using Akka.Cluster.Sharding;
using GridDomain.Aggregates;
using GridDomain.Node.Akka.Actors;
using GridDomain.Node.Akka.Actors.Aggregates;
using GridDomain.Node.Akka.Cluster;

namespace GridDomain.Node
{
    public class ShardEnvelop : IShardEnvelop
    {
        public string EntityId { get; protected set; }
        public string ShardId { get; protected set; }
        public string Region { get; }
        public object Message { get; protected set; }

        public ShardEnvelop(object message, string shardId, string entityId, string region)
        {
            Message = message;
            ShardId = shardId;
            EntityId = entityId;
            Region = region;
        }
    }

    public interface IShardEnvelop
    {
        string EntityId { get; }
        string ShardId { get; }
        string Region { get; }
        object Message { get; }
    }

    public class ShardedPassivate : IShardEnvelop
    {
        public ShardedPassivate(AggregateAddress aggregate)
        {
            ShardId = ShardIdGenerator.Instance.GetShardId(aggregate.Id);
            Region = aggregate.Name;
            EntityId = aggregate.ToString();
        }

        public string EntityId { get; }
        public string ShardId { get; }
        public string Region { get; }
        public object Message { get; } = new Passivate(AggregateActor.ShutdownGratefully.Instance);
    }

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

        public static ShardedAggregateCommand New(ICommand message, IMessageMetadata metadata = null)
        {
            return new ShardedAggregateCommand(new AggregateActor.ExecuteCommand(message, metadata), message.Recipient.ToString(),
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