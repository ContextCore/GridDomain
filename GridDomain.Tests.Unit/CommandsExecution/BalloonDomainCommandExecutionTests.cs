using GridDomain.Tests.Unit.BalloonDomain;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class BalloonDomainCommandExecutionTests : NodeTestKit
    {
        public BalloonDomainCommandExecutionTests(ITestOutputHelper output)
            : base(output, new NodeTestFixture(new BalloonDomainConfiguration())) {}
    }
}