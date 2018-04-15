using GridDomain.Tests.Unit.ProcessManagers;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.ProcessManagers
{
    public class Cluster_Given_uninitialized_process_When_processing_messages : Given_uninitialized_process_When_processing_messages
    {
        public Cluster_Given_uninitialized_process_When_processing_messages(ITestOutputHelper helper)
            : base(new SoftwareProgrammingProcessManagerFixture(helper).Clustered()) { }
    }
}