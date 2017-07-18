using GridDomain.Tests.Unit.DependencyInjection.FutureEvents.Configuration;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.DependencyInjection.FutureEvents
{
    internal class FutureEventsFixture : NodeTestFixture
    {
        public FutureEventsFixture(ITestOutputHelper output = null) : base(null, null, output)
        {
            Add(new FutureAggregateDomainConfiguration());
        }
    }
}