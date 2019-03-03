using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Aggregates;
using GridDomain.Node.Akka.Actors.Aggregates;

namespace GridDomain.Node.Akka.Cluster
{
    public class ClusterAggregatesLifetime : IAggregatesLifetime
    {
        private readonly IActorRef _aggregatesRouter;
        private readonly ActorSystem _system;

        public ClusterAggregatesLifetime(ActorSystem system, IActorRef aggregatesRouter)
        {
            _system = system;
            _aggregatesRouter = aggregatesRouter;
        }

        public async Task<AggregateHealthReport> GetHealth(IAggregateAddress aggregate, TimeSpan? timeout = null)
        {
            var shardId = ShardIdGenerator.Instance.GetShardId(aggregate.Id);
            var region = aggregate.Name;
            var message = new ShardEnvelop(AggregateActor.CheckHealth.Instance, shardId,
                aggregate.ToString(), region);

            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var report =
                        await _aggregatesRouter.Ask<AggregateHealthReport>(message, timeout ?? TimeSpan.FromSeconds(2));
                    return report;
                }
                catch (AskTimeoutException ex)
                {
                    continue;
                }
            }

            throw new TimeoutException();
        }
    }
}