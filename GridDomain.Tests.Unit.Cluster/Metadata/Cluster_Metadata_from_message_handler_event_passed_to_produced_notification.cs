using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.Metadata;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.Metadata
{
    public class Cluster_Metadata_from_message_handler_event_passed_to_produced_notification : Metadata_from_message_handler_event_passed_to_produced_notification
    {
        public Cluster_Metadata_from_message_handler_event_passed_to_produced_notification(ITestOutputHelper output) 
            : base(new NodeTestFixture(output).Add(new BalloonDomainConfiguration())) { }

    }
}