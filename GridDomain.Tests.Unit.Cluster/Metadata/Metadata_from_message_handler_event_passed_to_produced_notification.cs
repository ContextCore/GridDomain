using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Metadata
{
    public class Cluster_Metadata_from_message_handler_event_passed_to_produced_notification : Metadata_from_message_handler_event_passed_to_produced_notification
    {
        public Cluster_Metadata_from_message_handler_event_passed_to_produced_notification(ITestOutputHelper output) 
            : base(new NodeTestFixture(output).Add(new BalloonDomainConfiguration())) { }

    }
}