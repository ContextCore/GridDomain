using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.FutureEvents
{
    public class FutureEventsTest : NodeTestKit
    {
        public FutureEventsTest(ITestOutputHelper output) : base(output, new FutureEventsFixture()) {}
    }
}