using GridDomain.Node;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents;
using Microsoft.Practices.Unity;
using Quartz;

namespace GridDomain.Tests.Unit.EventsUpgrade
{
    public class BalanceFixture : NodeTestFixture
    {
        public BalanceFixture()
        {
            Add(new BalanceDomainDonfiguration());
            this.ClearSheduledJobs();
        }

        protected override NodeSettings CreateNodeSettings()
        {
            var nodeSettings = base.CreateNodeSettings();
            nodeSettings.QuartzJobRetrySettings = new InMemoryRetrySettings(1, null, new NeverRetryExceptionPolicy());
            return nodeSettings;
        }
    }
}