using System;
using Akka.Cluster;
using GridDomain.Balance.Node;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Tests.Acceptance.Persistence;

namespace GridDomain.Tests.Acceptance.Balance.ReadModelConcurrentBuild
{
    public class Cluster_Given_balance_change_plan_When_executing: Given_balance_change_plan_When_executing
    {
        protected override GridDomainNode GreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            var actorSystems = ActorSystemFactory.CreateCluster(akkaConf).RandomElement();
            var cluster = Akka.Cluster.Cluster.Get(actorSystems);
            cluster.Subscribe(TestActor, new[] { typeof(ClusterEvent.IReachabilityEvent) });

            return new GridDomainNode(DefaultUnityContainer(dbConfig), 
                                      new BalanceCommandsRouting(), 
                                      actorSystems,
                                      TransportMode.Cluster);
        }

        public Cluster_Given_balance_change_plan_When_executing() : base("")
        {
          
        }

        protected override void AfterCommandExecuted()
        {
            var msg = ExpectMsg<ClusterEvent.IReachabilityEvent>();
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(10);
        protected override int BusinessNum => 1;
        protected override int ChangesPerBusiness => 1;
    }
}