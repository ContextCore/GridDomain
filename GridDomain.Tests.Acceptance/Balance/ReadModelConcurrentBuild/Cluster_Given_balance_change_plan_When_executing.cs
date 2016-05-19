using System.Linq;
using GridDomain.Balance.Node;
using GridDomain.Node;
using GridDomain.Node.Configuration;
using Microsoft.Practices.Unity;

namespace GridDomain.Tests.Acceptance.ReadModelConcurrentBuild
{
    public class Cluster_Given_balance_change_plan_When_executing :
        Single_System_Given_balance_change_plan_When_executing
    {

        protected override GridDomainNode GreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            return new GridDomainNode(DefaultUnityContainer(dbConfig), new BalanceCommandsRouting(), ActorSystemFactory.CreateActorSystem(akkaConf));
        }
    }
}