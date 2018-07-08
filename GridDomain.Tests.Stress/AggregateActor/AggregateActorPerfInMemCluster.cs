using Akka.Actor;
using GridDomain.Node.Cluster;
using Xunit.Abstractions;

namespace GridDomain.Tests.Stress.AggregateActor {
    public class AggregateActorPerfInMemCluster : AggregateActorPerf
    {
        public AggregateActorPerfInMemCluster(ITestOutputHelper output):base(output,"") 
        {
            
        }

        protected override ActorSystem CreateActorSystem()
        {
            return new StressTestNodeConfiguration().ToCluster().Result.Cluster.System;
        }
    }
}