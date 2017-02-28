using GridDomain.Tests.XUnit.SampleDomain;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class SampleDomainCommandExecutionTests : NodeTestKit
    {
        public SampleDomainCommandExecutionTests(ITestOutputHelper output)
            : base(output, new NodeTestFixture(new SampleDomainContainerConfiguration(), new SampleRouteMap())) {}
    }
}