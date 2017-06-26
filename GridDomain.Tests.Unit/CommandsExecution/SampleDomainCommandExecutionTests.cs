using GridDomain.Tests.Unit.BalloonDomain;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class SampleDomainCommandExecutionTests : NodeTestKit
    {
        public SampleDomainCommandExecutionTests(ITestOutputHelper output)
            : base(output, new NodeTestFixture(new BalloonContainerConfiguration(), new BalloonRouteMap())) {}
    }
}