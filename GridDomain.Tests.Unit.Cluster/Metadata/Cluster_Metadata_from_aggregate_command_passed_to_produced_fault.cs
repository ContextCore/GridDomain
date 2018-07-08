using GridDomain.Tests.Unit.Metadata;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.Metadata
{
    public class Cluster_Metadata_from_aggregate_command_passed_to_produced_fault : Metadata_from_aggregate_command_passed_to_produced_fault
    {
        public Cluster_Metadata_from_aggregate_command_passed_to_produced_fault(ITestOutputHelper output) 
            : base(new NodeTestFixture(output).Clustered()) {}
    }
}