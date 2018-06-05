using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.Cluster;
using GridDomain.Tests.Unit.EventsUpgrade.Domain;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.EventsUpgrade
{
    public class Cluster_Given_aggregate_with_upgraded_event_with_new_field : Given_aggregate_with_upgraded_event_with_new_field
    {
        public Cluster_Given_aggregate_with_upgraded_event_with_new_field(ITestOutputHelper output)
            : base(new BalanceFixture(output).Clustered().UseAdaper(new BalanceChangedDomainEventAdapter1())) {}

    }
}