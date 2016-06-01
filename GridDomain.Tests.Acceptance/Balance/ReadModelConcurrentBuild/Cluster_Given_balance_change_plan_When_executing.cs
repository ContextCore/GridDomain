using System;
using System.CodeDom;
using System.Linq;
using Akka.Actor;
using Akka.DI.Unity;
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
            var akkaCluster = ActorSystemFactory.CreateCluster(akkaConf);
            var container = DefaultUnityContainer(dbConfig);

            foreach (var system in akkaCluster.All)
            {
                var dependencyResolver = new UnityDependencyResolver(container, system);
            }

            return new GridDomainNode(container, 
                                      new BalanceCommandsRouting(),
                                      TransportMode.Cluster, akkaCluster.All);
        }

        public Cluster_Given_balance_change_plan_When_executing() : base("")
        {
          
        }

        protected override void AfterCommandExecuted()
        {
            //cluster.Subscribe(TestActor, new[] { typeof(ClusterEvent.IReachabilityEvent) });
            //this.FishForMessage<>()
            //var msg = ExpectMsgAnyOf(typeof(ClusterEvent.IReachabilityEvent),
            //                         typeof(ClusterEvent.IClusterDomainEvent),
            //                         typeof(ClusterEvent.CurrentClusterState));
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(10);
        protected override int BusinessNum => 1;
        protected override int ChangesPerBusiness => 1;
    }
}