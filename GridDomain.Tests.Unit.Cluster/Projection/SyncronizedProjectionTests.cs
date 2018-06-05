using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.SyncProjection;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.Projection
{
    public class ClusterSynchronizedProjectionBuildersTests : SynchronizedProjectionBuildersTests
    {
        protected ClusterSynchronizedProjectionBuildersTests(ITestOutputHelper output)
            : base(new NodeTestFixture(output).Clustered().Add(new BalloonDomainConfiguration())) {}

    }
}