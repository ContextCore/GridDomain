using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain;

namespace GridDomain.Tests.XUnit.EventsUpgrade
{
    public class BalanceFixture : NodeTestFixture
    {
        public BalanceFixture()
        {
            Add(new CustomContainerConfiguration(c => c.RegisterAggregate<BalanceAggregate, BalanceAggregatesCommandHandler>()));
            Add(new BalanceRouteMap());
        }

        protected override NodeSettings CreateNodeSettings()
        {
            var nodeSettings = base.CreateNodeSettings();
            nodeSettings.QuartzJobRetrySettings = new InMemoryRetrySettings(1,null,new NeverRetryExceptionPolicy());
            return nodeSettings;
        }
    }
}