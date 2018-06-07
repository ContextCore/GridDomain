using GridDomain.Tests.Unit.FutureEvents;
using GridDomain.Tests.Unit.Metadata;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.Metadata
{
    public class Cluster_Metadata_from_command_passed_to_produced_scheduled_event : Metadata_from_command_passed_to_produced_scheduled_event
    {
        public Cluster_Metadata_from_command_passed_to_produced_scheduled_event(ITestOutputHelper output) 
            : base(new FutureEventsFixture(output).Clustered()) {}
    }
}