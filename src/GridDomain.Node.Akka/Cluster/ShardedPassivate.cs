using Akka.Cluster.Sharding;
using GridDomain.Aggregates;
using GridDomain.Node.Akka.Actors.Aggregates;
using GridDomain.Node.Akka.Cluster;

namespace GridDomain.Node.Akka
{
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
}