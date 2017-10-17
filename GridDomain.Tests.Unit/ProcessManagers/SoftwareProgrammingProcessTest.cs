using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers
{
    public class SoftwareProgrammingProcessTest : NodeTestKit
    {
        public SoftwareProgrammingProcessTest(ITestOutputHelper helper)
            : base(new SoftwareProgrammingProcessManagerFixture(helper)) {}
    }
}