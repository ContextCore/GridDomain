using GridDomain.Tests.Unit.EventsUpgrade;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Cluster.EventsUpgrade
{
    public class Cluster_Given_aggregate_with_upgraded_event_with_new_field : Given_aggregate_with_upgraded_event_with_new_field
    {
        public Cluster_Given_aggregate_with_upgraded_event_with_new_field(ITestOutputHelper output)
            : base(new BalanceFixture(output).Clustered().UseAdaper(new BalanceChangedDomainEventAdapter1())) {}

    }
}