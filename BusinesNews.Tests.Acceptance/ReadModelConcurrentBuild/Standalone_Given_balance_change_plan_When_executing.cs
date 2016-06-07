using System;
using BusinessNews.Node;
using GridDomain.Node;
using GridDomain.Node.Configuration;

namespace BusinesNews.Tests.Acceptance.ReadModelConcurrentBuild
{
    public abstract class Standalone_Given_balance_change_plan_When_executing : Given_balance_change_plan_When_executing
    {
        protected override GridDomainNode GreateGridDomainNode(AkkaConfiguration akkaConf, IDbConfiguration dbConfig)
        {
            return new GridDomainNode(CreateUnityContainer(dbConfig),
                                     new BusinessNewsRouting(),
                                     TransportMode.Standalone, ActorSystemFactory.CreateActorSystem(akkaConf));
        }

        public Standalone_Given_balance_change_plan_When_executing(): base(AkkaConf.ToStandAloneSystemConfig())
        {
        }
    }

    public class Standalone_Smoke_Given_balance_change_plan_When_executing : Standalone_Given_balance_change_plan_When_executing
    {
        protected override TimeSpan Timeout => TimeSpan.FromSeconds(3);
        protected override int BusinessNum => 1;
        protected override int ChangesPerBusiness => 1;
    }

    public class Standalone_Load_Given_balance_change_plan_When_executing : Standalone_Given_balance_change_plan_When_executing
    {
        protected override int BusinessNum => 10;
        protected override int ChangesPerBusiness => 10;
    }
}