using GridDomain.Tests.Unit.FutureEvents;
using GridDomain.Tests.Unit.Metadata;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.Metadata
{
    public class Cluster_Metadata_from_command_passed_to_produced_scheduled_fault : Metadata_from_command_passed_to_produced_scheduled_fault
    {
        public Cluster_Metadata_from_command_passed_to_produced_scheduled_fault(ITestOutputHelper output)
            : base(new FutureEventsFixture(output).Clustered()) {}
    }
}