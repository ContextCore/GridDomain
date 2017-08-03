using GridDomain.Scheduling.Quartz.Configuration;
using GridDomain.Scheduling.Quartz.Retry;
using GridDomain.Tests.Unit.FutureEvents.Configuration;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents
{
    internal class FutureEventsFixture : NodeTestFixture
    {
        public FutureEventsFixture(ITestOutputHelper output = null) : base(null, null, output)
        {
            Add(new FutureAggregateDomainConfiguration());
        }
    }
}