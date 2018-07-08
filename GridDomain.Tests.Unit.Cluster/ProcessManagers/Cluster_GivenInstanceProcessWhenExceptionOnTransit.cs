using GridDomain.Tests.Unit.ProcessManagers;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.ProcessManagers
{
    public class Cluster_GivenInstanceProcessWhenExceptionOnTransit : GivenInstanceProcessWhenExceptionOnTransit
    {
        public Cluster_GivenInstanceProcessWhenExceptionOnTransit(ITestOutputHelper helper) : base(new SoftwareProgrammingProcessManagerFixture(helper).Clustered()) {}
    }
}