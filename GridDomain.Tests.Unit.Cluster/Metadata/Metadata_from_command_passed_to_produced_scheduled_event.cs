using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.FutureEvents;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Metadata
{
    public class Cluster_Metadata_from_command_passed_to_produced_scheduled_event : Metadata_from_command_passed_to_produced_scheduled_event
    {
        public Cluster_Metadata_from_command_passed_to_produced_scheduled_event(ITestOutputHelper output) 
            : base(new NodeTestFixture(output).Clustered()) {}
    }
}