using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Scheduling;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.FutureEvents.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.FutureEvents
{
    public class Cluster_Given_different_aggregates_future_events_Should_be_processed_independently : Given_different_aggregates_future_events_Should_be_processed_independently
    {
        public Cluster_Given_different_aggregates_future_events_Should_be_processed_independently(ITestOutputHelper output)
            : base(new FutureEventsFixture(output).Clustered()) { }
    }
}