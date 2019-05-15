using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Aggregates;
using GridDomain.Domains;
using GridDomain.Node.Akka.Actors.Aggregates;

namespace GridDomain.Node.Akka.Cluster
{
    public class ClusterAggregatesController : IAggregatesController
    {
        private readonly IActorRef _aggregatesRouter;

        public ClusterAggregatesController(ActorSystem system, IActorRef aggregatesRouter)
        {
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
                catch (AskTimeoutException)
                {
                }
            }

            throw new TimeoutException();
        }
    }
}