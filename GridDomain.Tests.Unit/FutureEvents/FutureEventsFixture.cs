using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Unit.FutureEvents.Configuration;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents
{
    internal class FutureEventsFixture : NodeTestFixture
    {
        public FutureEventsFixture(ITestOutputHelper output=null) : this(output,(IQuartzConfig)null)
        {
        }
        public FutureEventsFixture(ITestOutputHelper output, IRetrySettings settings) : base(null, null, output)
        {
            Add(new FutureAggregateDomainConfiguration());
            this.EnableScheduling(settings);
        }
        public FutureEventsFixture(ITestOutputHelper output, IQuartzConfig config, bool clearScheduledData = true) : base(null, null, output)
        {
            Add(new FutureAggregateDomainConfiguration());
            this.EnableScheduling(config,clearScheduledData);
        }
    }
}