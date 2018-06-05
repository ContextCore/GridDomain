using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Hadlers;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.Unit.Cluster;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Metadata
{
    public class Cluster_Metadata_from_message_handler_event_passed_to_produced_fault : Metadata_from_message_handler_event_passed_to_produced_fault
    {
        public Cluster_Metadata_from_message_handler_event_passed_to_produced_fault(ITestOutputHelper output)
            : base(new NodeTestFixture(output, new[] {new FaultyBalloonProjectionDomainConfiguration()}).Clustered()) { }
    }

}