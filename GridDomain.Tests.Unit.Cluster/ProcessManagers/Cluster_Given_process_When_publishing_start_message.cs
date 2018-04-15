using GridDomain.Tests.Unit.ProcessManagers;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.ProcessManagers
{
    public class Cluster_Given_process_When_publishing_start_message : Given_process_When_publishing_start_message
    {
        public Cluster_Given_process_When_publishing_start_message(ITestOutputHelper helper) :
            base(new SoftwareProgrammingProcessManagerFixture(helper).Clustered()) { }
    }
}