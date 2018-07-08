using GridDomain.Tests.Acceptance.Scheduling;
using GridDomain.Tests.Unit.Cluster;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Cluster.Scheduling
{
    public class Cluster_NodeSchedulerTests : NodeSchedulerTests
    {
        public Cluster_NodeSchedulerTests(ITestOutputHelper output)
            : base(new SchedulerFixture(output).Clustered()) { }
    }
}