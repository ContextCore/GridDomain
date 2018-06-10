using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Scheduling;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.FutureEvents;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.FutureDomainEvents
{
    public class Cluster_Given_aggregate_When_raising_several_future_events_Persistent : Given_aggregate_When_raising_several_future_events_Persistent
    {
        public Cluster_Given_aggregate_When_raising_several_future_events_Persistent(ITestOutputHelper output) 
            : base(new FutureEventsFixture(output).Clustered()) { }

    }
}