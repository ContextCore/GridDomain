using System;
using GridDomain.Node;
using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Common;
using Quartz;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.EventsUpgrade
{
    public class BalanceFixture : NodeTestFixture
    {
        protected readonly BalanceDomainDonfiguration BalanceDomainDonfiguration;

        public BalanceFixture(ITestOutputHelper output,IQuartzConfig config = null) :base(output)
        {
            BalanceDomainDonfiguration = new BalanceDomainDonfiguration();
            this.EnableScheduling(config ?? new InMemoryQuartzConfig(new InMemoryRetrySettings(1, null, new NeverRetryExceptionPolicy())));
            Add(BalanceDomainDonfiguration);
        }

      
        public BalanceFixture InitFastRecycle(
            TimeSpan? clearPeriod = null,
            TimeSpan? maxInactiveTime = null)
        {
            this.BalanceDomainDonfiguration.DefaultAggregateDependencyFactory.RecycleConfigurationCreator = () =>
                                                                                                                new RecycleConfiguration(clearPeriod ?? TimeSpan.FromMilliseconds(200),
                                                                                                                    maxInactiveTime ?? TimeSpan.FromMilliseconds(50));
            return this;
        }
    }
}