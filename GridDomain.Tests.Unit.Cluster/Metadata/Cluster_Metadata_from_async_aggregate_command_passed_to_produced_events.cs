using GridDomain.Tests.Unit.Metadata;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.Metadata
{
    public class Cluster_Metadata_from_async_aggregate_command_passed_to_produced_events : Metadata_from_async_aggregate_command_passed_to_produced_events
    {
        public Cluster_Metadata_from_async_aggregate_command_passed_to_produced_events(ITestOutputHelper output) 
            : base(new NodeTestFixture(output).Clustered().LogToFile(nameof(Cluster_Metadata_from_async_aggregate_command_passed_to_produced_events))) {}
    }
}