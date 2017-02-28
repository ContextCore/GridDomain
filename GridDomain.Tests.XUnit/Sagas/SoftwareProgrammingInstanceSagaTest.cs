using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas
{
    public class SoftwareProgrammingInstanceSagaTest : NodeTestKit
    {
        public SoftwareProgrammingInstanceSagaTest(ITestOutputHelper helper)
            : base(helper, new SoftwareProgrammingSagaFixture()) {}
    }
}