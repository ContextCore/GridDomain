using GridDomain.Aggregates;
using GridDomain.Node.Akka.Actors;
using GridDomain.Node.Akka.Actors.Aggregates;
using GridDomain.Node.Akka.Cluster;

namespace GridDomain.Node
{
    
//    public class ShardEnvelop:IShardEnvelop
//    {
//        public string EntityId { get; protected set; }
//        public string ShardId { get; protected set; }
//        public string Region { get; }
//        public object Message { get; protected set; }
//
//        public ShardEnvelop(object message, string shardId, string entityId, string region)
//        {
//            Message = message;
//            ShardId = shardId;
//            EntityId = entityId;
//            Region = region;
//        }
//    }
    public interface IShardEnvelop
    {
         string EntityId { get;}
         string ShardId { get;}
         string Region { get; }
         object Message { get; }
    }

    public class ShardedAggregateCommand: IShardEnvelop, IHaveMetadata
    {
        public ShardedAggregateCommand(ICommand command, IMessageMetadata metadata = null, IShardIdGenerator generator=null)
        {
            Metadata = metadata ?? MessageMetadata.Empty;
            generator = generator ?? DefaultShardIdGenerator.Instance;
            ShardId = generator.GetShardId(command.AggregateId);
            EntityId = EntityActorName.GetFullName(command.AggregateType, command.AggregateId);
            Region = Known.Names.Region(command.AggregateType);
            Message = new AggregateActor.ExecuteCommand(command,Metadata);
        }

        public IMessageMetadata Metadata { get; }
        public string EntityId { get; }
        public string ShardId { get; }
        public string Region { get; }
        public object Message { get; }
    }
}