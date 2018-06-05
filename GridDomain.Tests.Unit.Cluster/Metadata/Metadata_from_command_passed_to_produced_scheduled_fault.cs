using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.FutureEvents;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Metadata
{
    public class Cluster_Metadata_from_command_passed_to_produced_scheduled_fault : Metadata_from_command_passed_to_produced_scheduled_fault
    {
        public Cluster_Metadata_from_command_passed_to_produced_scheduled_fault(ITestOutputHelper output)
            : base(new FutureEventsFixture(output).Clustered()) {}
    }
}