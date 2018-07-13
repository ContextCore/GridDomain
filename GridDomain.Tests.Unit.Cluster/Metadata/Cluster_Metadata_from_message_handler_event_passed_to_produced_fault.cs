using GridDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.Unit.Metadata;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.Metadata
{
    public class Cluster_Metadata_from_message_handler_event_passed_to_produced_fault : Metadata_from_message_handler_event_passed_to_produced_fault
    {
        public Cluster_Metadata_from_message_handler_event_passed_to_produced_fault(ITestOutputHelper output)
            : base(new NodeTestFixture(output, new FaultyBalloonProjectionDomainConfiguration()).Clustered()) { }
    }

}