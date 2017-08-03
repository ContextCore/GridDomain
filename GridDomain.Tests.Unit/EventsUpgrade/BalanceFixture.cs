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

        public BalanceFixture(IQuartzConfig config = null) 
        {
            BalanceDomainDonfiguration = new BalanceDomainDonfiguration();
            this.EnableScheduling(config ?? new InMemoryQuartzConfig(new InMemoryRetrySettings(1, null, new NeverRetryExceptionPolicy())));

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