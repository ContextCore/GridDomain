using GridDomain.Node;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Unit.EventsUpgrade.Domain;
using Microsoft.Practices.Unity;
using Quartz;

namespace GridDomain.Tests.Unit.EventsUpgrade
{
    public class BalanceFixture : NodeTestFixture
    {
        public BalanceFixture()
        {
            Add(
                new ContainerConfiguration(
                                                 c =>
                                                 {
                                                     c.Register(AggregateConfiguration.New<BalanceAggregate, BalanceAggregatesCommandHandler>());
                                                 }));
            Add(new BalanceRouteMap());
            OnNodeStartedEvent += (sender, args) => Node.Container.Resolve<IScheduler>().Clear();
        }

        protected override NodeSettings CreateNodeSettings()
        {
            var nodeSettings = base.CreateNodeSettings();
            nodeSettings.QuartzJobRetrySettings = new InMemoryRetrySettings(1, null, new NeverRetryExceptionPolicy());
            return nodeSettings;
        }
    }
}