using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors
{
    public class BalloonDomainCommandExecutionTests : NodeTestKit
    {
        protected BalloonDomainCommandExecutionTests(ITestOutputHelper output)
            : base(new NodeTestFixture(output,new BalloonDomainConfiguration())) {}
        
        public BalloonDomainCommandExecutionTests(NodeTestFixture fixture)
            : base(fixture) {}
    }
}