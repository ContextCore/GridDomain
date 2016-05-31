using System;
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
            return new GridDomainNode(DefaultUnityContainer(dbConfig), 
                                      new BalanceCommandsRouting(), 
                                      ActorSystemFactory.CreateCluster(akkaConf).RandomElement(),
                                      TransportMode.Cluster);
        }

        public Cluster_Given_balance_change_plan_When_executing() : base("")
        {
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(10);
    }
}