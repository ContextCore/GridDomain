using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents
{
    public class Cluster_Given_aggregate_When_raising_future_event_by_commands : Given_aggregate_When_raising_future_event_by_commands
    {
        public Cluster_Given_aggregate_When_raising_future_event_by_commands(ITestOutputHelper output)
            : base(new FutureEventsFixture(output).Clustered()) { }
    }
}