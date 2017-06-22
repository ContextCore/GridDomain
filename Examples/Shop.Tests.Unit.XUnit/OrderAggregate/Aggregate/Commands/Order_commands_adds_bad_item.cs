using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NMoneys;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Commands;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.OrderAggregate.Exceptions;
using Shop.Infrastructure;
using Xunit;

namespace Shop.Tests.Unit.XUnit.OrderAggregate.Aggregate.Commands
{
   
    public class Order_commands_adds_bad_item : AggregateCommandsTest<Order, OrderCommandsHandler>
    {
        protected override OrderCommandsHandler CreateCommandsHandler()
        {
            return new OrderCommandsHandler(new InMemorySequenceProvider());
        }

        public Order_commands_adds_bad_item() //Given_empty_order()
        {
            Init();
        }

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new OrderCreated(Guid.NewGuid(), 123, Guid.NewGuid());
        }

       [Fact]
        public void Order_should_throw_exñeption_on_item_add_with_negative_money()
        {
            Assert.ThrowsAsync<InvalidMoneyException>(async () =>
                                                      {
                                                          var cmd = new AddItemToOrderCommand(Aggregate.Id,
                                                                                              Guid.NewGuid(),
                                                                                              20,
                                                                                              new Money(-123));
                                                          await Execute(cmd);
                                                      });
        }

       [Fact]
        public void Order_should_throw_exñeption_on_item_add_with_negative_quantity()
        {
            Assert.ThrowsAsync<InvalidQuantityException>(async () =>
                                                         {
                                                             var cmd = new AddItemToOrderCommand(Aggregate.Id,
                                                                                                 Guid.NewGuid(),
                                                                                                 -1,
                                                                                                 new Money(123));
                                                             await Execute(cmd);
                                                         });
        }
    }
}