using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
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
   
    public class Completed_order_test : AggregateCommandsTest<Order, OrderCommandsHandler>
    {
        public Completed_order_test() //Given_completed_order()
        {
            Init();
        }

        protected override OrderCommandsHandler CreateCommandsHandler()
        {
            return new OrderCommandsHandler(new InMemorySequenceProvider());
        }

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new OrderCreated(Aggregate.Id, 123, Guid.NewGuid());
            yield return new OrderCompleted(Aggregate.Id, OrderStatus.Paid);
        }

       [Fact]
        public void When_completed_order_try_add_items_it_throws_exeption()
        {
            Assert.Throws<CantAddItemsToClosedOrder>(() =>
                                                     {
                                                         var cmd = new AddItemToOrderCommand(Aggregate.Id,
                                                                                             Guid.NewGuid(),
                                                                                             123,
                                                                                             new Money(1));
                                                         Execute(cmd);
                                                     });
        }

       [Fact]
        public async Task When_completed_order_try_complete_it_throws_exeption()
        {
            await Assert.ThrowsAsync<CannotCompleteAlreadyClosedOrderException>(() => Execute(new CompleteOrderCommand(Aggregate.Id)));
        }

       [Fact]
        public void When_order_completes_it_chages_status_to_paid()
        {
            Assert.Equal(OrderStatus.Paid, Aggregate.Status);
        }
    }
}