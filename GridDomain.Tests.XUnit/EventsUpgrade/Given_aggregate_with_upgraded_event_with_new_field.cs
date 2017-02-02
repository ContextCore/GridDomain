using System;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.CQRS.Messaging;
using GridDomain.EventSourcing;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain;
using GridDomain.Tests.XUnit.EventsUpgrade.Domain.Events;
using Microsoft.Practices.Unity;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.EventsUpgrade
{
    
    public class Given_aggregate_with_upgraded_event_with_new_field: NodeTestKit
    {

        public Given_aggregate_with_upgraded_event_with_new_field(ITestOutputHelper output) : base(output, new EventsUpgradeFixture())
        {
        }

        [Fact]
        public async Task When_aggregate_is_recovered_from_persistence()
        {
            var balanceId = Guid.NewGuid();
            var events = new DomainEvent[]
            {
                new AggregateCreatedEvent(0, balanceId),
                new BalanceChangedEvent_V0(5, balanceId,2),
                new BalanceChangedEvent_V1(5, balanceId),
            };

             await Node.System.SaveToJournal<BalanceAggregate>(balanceId, events);
             var aggregate = await this.LoadAggregate<BalanceAggregate>(balanceId);

            //Then_it_should_process_old_and_new_event()
            Assert.Equal(15, aggregate.Amount);
        }

        class EventsUpgradeFixture : NodeTestFixture
        {
            protected override void OnNodeCreated()
            {
                Node.EventsAdaptersCatalog.Register(new BalanceChangedDomainEventAdapter1());
            }

            protected override IContainerConfiguration CreateContainerConfiguration()
            {
                return
                    new CustomContainerConfiguration(
                        c => c.RegisterAggregate<BalanceAggregate, BalanceAggregatesCommandHandler>(),
                        c => c.RegisterInstance<IQuartzConfig>(new InMemoryQuartzConfig()));
            }

            protected override IMessageRouteMap CreateRouteMap()
            {
                return new BalanceRouteMap();
            }
        }

    }
}