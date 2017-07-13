using System;
using GridDomain.Node;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.DependencyInjection.FutureEvents;
using Microsoft.Practices.Unity;
using Quartz;

namespace GridDomain.Tests.Unit.EventsUpgrade
{
    public class BalanceFixture : NodeTestFixture
    {
        protected readonly BalanceDomainDonfiguration BalanceDomainDonfiguration;

        public BalanceFixture() : base()
        {
            BalanceDomainDonfiguration = new BalanceDomainDonfiguration();
            this.ClearSheduledJobs();
        }

        protected override NodeSettings CreateNodeSettings()
        {
            Add(BalanceDomainDonfiguration);
            var nodeSettings = base.CreateNodeSettings();
            nodeSettings.QuartzConfig.RetryOptions = new InMemoryRetrySettings(1, null, new NeverRetryExceptionPolicy());
            return nodeSettings;
        }
        public BalanceFixture InitFastRecycle(
            TimeSpan? clearPeriod = null,
            TimeSpan? maxInactiveTime = null)
        {
            this.BalanceDomainDonfiguration.DefaultAggregateDependencyFactory.RecycleConfigurationCreator = () =>
                                                                                                                new PersistentChildsRecycleConfiguration(clearPeriod ?? TimeSpan.FromMilliseconds(200),
                                                                                                                    maxInactiveTime ?? TimeSpan.FromMilliseconds(50));
            return this;
        }
    }
}