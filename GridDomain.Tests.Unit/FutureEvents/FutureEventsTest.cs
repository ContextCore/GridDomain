using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents
{
    public class FutureEventsTest : NodeTestKit
    {
        public FutureEventsTest(ITestOutputHelper output) : base(new FutureEventsFixture(output)) {}
    }
}