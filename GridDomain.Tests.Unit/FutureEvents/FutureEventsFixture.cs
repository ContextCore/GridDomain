using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Unit.FutureEvents.Configuration;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents
{
    public class FutureEventsFixture : NodeTestFixture
    {
        public FutureEventsFixture(ITestOutputHelper output, IRetrySettings settings=null) : base(output)
        {
            Add(new FutureAggregateDomainConfiguration());
            this.EnableScheduling(settings ?? new InMemoryRetrySettings());
        }
        public FutureEventsFixture(ITestOutputHelper output, IQuartzConfig config, bool clearScheduledData = true) : base(output)
        {
            Add(new FutureAggregateDomainConfiguration());
            this.EnableScheduling(config,clearScheduledData);
        }
    }
}