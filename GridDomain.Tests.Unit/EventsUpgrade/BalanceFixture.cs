using System;
using GridDomain.Node;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Common;
using Microsoft.Practices.Unity;
using Quartz;

namespace GridDomain.Tests.Unit.EventsUpgrade
{
    public class BalanceFixture : NodeTestFixture
    {
        protected readonly BalanceDomainDonfiguration BalanceDomainDonfiguration;

        private BalanceFixture() 
        {
            BalanceDomainDonfiguration = new BalanceDomainDonfiguration();
        }

        public static BalanceFixture NewInMemory()
        {
            var f = new BalanceFixture();
            f.EnableScheduling(new InMemoryQuartzConfig(new InMemoryRetrySettings(1, null, new NeverRetryExceptionPolicy())));
            f.ClearSheduledJobs();
            return f;
        }

        protected override NodeSettings CreateNodeSettings()
        {
            Add(BalanceDomainDonfiguration);
            var nodeSettings = base.CreateNodeSettings();
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