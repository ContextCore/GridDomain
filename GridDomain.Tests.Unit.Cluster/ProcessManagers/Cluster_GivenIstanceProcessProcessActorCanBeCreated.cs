using GridDomain.Tests.Unit.ProcessManagers;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.ProcessManagers
{
    public class Cluster_GivenIstanceProcessProcessActorCanBeCreated : GivenIstanceProcessProcessActorCanBeCreated
    {
        public Cluster_GivenIstanceProcessProcessActorCanBeCreated(ITestOutputHelper helper) : base(new SoftwareProgrammingProcessManagerFixture(helper).Clustered()) {}

    }
}