using BusinessNews.Node;
using GridDomain.Node;
using GridDomain.Node.Configuration;

namespace BusinesNews.Tests.Acceptance.ReadModelConcurrentBuild
{
    public class Standalne_Given_balance_change_plan_When_executing : Given_balance_change_plan_When_executing
    {
        protected override GridDomainNode GreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            return new GridDomainNode(CreateUnityContainer(dbConfig),
                new BusinessNewsRouting(),
                TransportMode.Standalone, ActorSystemFactory.CreateActorSystem(akkaConf));
        }

        public Standalne_Given_balance_change_plan_When_executing() : base(AkkaConf.ToStandAloneSystemConfig())
        {
        }

        protected override int BusinessNum => 10;
        protected override int ChangesPerBusiness => 10;
    }
}