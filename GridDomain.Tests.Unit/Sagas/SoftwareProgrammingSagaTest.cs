using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Sagas
{
    public class SoftwareProgrammingSagaTest : NodeTestKit
    {
        public SoftwareProgrammingSagaTest(ITestOutputHelper helper)
            : base(helper, new SoftwareProgrammingSagaFixture()) {}
    }
}