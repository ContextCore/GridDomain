using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class SoftwareProgrammingSagaTest : NodeTestKit
    {
        public SoftwareProgrammingSagaTest(ITestOutputHelper helper)
            : base(helper, new SoftwareProgrammingSagaFixture()) {}
    }
}