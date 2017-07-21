using System;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.EventsUpgrade.Domain;
using GridDomain.Tests.Unit.EventsUpgrade.Domain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.EventsUpgrade
{
    public class Given_aggregate_with_upgraded_event_with_new_field : NodeTestKit
    {
        public Given_aggregate_with_upgraded_event_with_new_field(ITestOutputHelper output)
            : base(output, new BalanceFixture().UseAdaper(new BalanceChangedDomainEventAdapter1())) {}

        protected Given_aggregate_with_upgraded_event_with_new_field(ITestOutputHelper output, NodeTestFixture fixture)
            : base(output, fixture) {}

        [Fact]
        public async Task When_aggregate_is_recovered_from_persistence()
        {
            var balanceId = Guid.NewGuid();
            var events = new DomainEvent[]
                         {
                             new AggregateCreatedEvent(0, balanceId),
                             new BalanceChangedEvent_V0(5, balanceId, 2),
                             new BalanceChangedEvent_V1(5, balanceId)
                         };

            await Node.SaveToJournal<BalanceAggregate>(balanceId, events);
            var aggregate = await Node.LoadAggregate<BalanceAggregate>(balanceId);

            //Then_it_should_process_old_and_new_event()
            Assert.Equal(15, aggregate.Amount);
        }
    }
}