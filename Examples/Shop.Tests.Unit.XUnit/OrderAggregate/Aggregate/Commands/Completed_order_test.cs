using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Common;
using NMoneys;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Commands;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.OrderAggregate.Exceptions;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Infrastructure;
using Xunit;

namespace Shop.Tests.Unit.XUnit.OrderAggregate.Aggregate.Commands
{
    public class Completed_order_tests
    {
        [Fact]
        public async Task When_completed_order_try_add_items_it_throws_exeption()
        {
            var scenario = new AggregateScenario<Order>(null, new OrderCommandsHandler(new InMemorySequenceProvider()));
            await scenario.Given(new OrderCreated(scenario.Id, 123, Guid.NewGuid()),
                                 new OrderCompleted(scenario.Id, OrderStatus.Paid))
                          .When(new AddItemToOrderCommand(scenario.Id,
                                                          Guid.NewGuid(),
                                                          123,
                                                          new Money(1)))
                          .RunAsync()
                          .ShouldThrow<CantAddItemsToClosedOrder>();
        }

        [Fact]
        public async Task When_completed_order_try_complete_it_throws_exeption()
        {
            var scenario = new AggregateScenario<Order>(null, new OrderCommandsHandler(new InMemorySequenceProvider()));
            await scenario.Given(new OrderCreated(scenario.Id, 123, Guid.NewGuid()),
                                 new OrderCompleted(scenario.Id, OrderStatus.Paid))
                          .When(new CompleteOrderCommand(scenario.Id))
                          .RunAsync()
                          .ShouldThrow<CannotCompleteAlreadyClosedOrderException>();
        }

        [Fact]
        public void When_order_completes_it_chages_status_to_paid()
        {
            var scenario = new AggregateScenario<Order>(null, new OrderCommandsHandler(new InMemorySequenceProvider()));
            scenario.Given(new OrderCreated(scenario.Id, 123, Guid.NewGuid()),
                           new OrderCompleted(scenario.Id, OrderStatus.Paid))
                    .Run();

            Assert.Equal(OrderStatus.Paid, scenario.Aggregate.Status);
        }
    }
}