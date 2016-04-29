using System.Linq;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using GridDomain.Node.MessageRouteConfigs;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.ReadModelConcurrentBuild
{
    public class Cluster_Given_balance_change_plan_When_executing :
        Single_System_Given_balance_change_plan_When_executing
    {
        protected override GridDomainNode GreateGridDomainNode(AkkaConfiguration akkaConf)
        {
            return new GridDomainNode(new UnityContainer(), new BalanceCommandsRouting(), ActorSystemFactory.CreateCluster(akkaConf).Last());
        }
    }
}