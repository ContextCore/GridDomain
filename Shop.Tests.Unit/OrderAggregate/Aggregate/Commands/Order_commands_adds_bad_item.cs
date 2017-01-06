using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NMoneys;
using NUnit.Framework;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Commands;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.OrderAggregate.Exceptions;
using Shop.Infrastructure;

namespace Shop.Tests.Unit.OrderAggregate.Aggregate.Commands
{
    [TestFixture]
    public class Order_commands_adds_bad_item : AggregateCommandsTest<Order,OrderCommandsHandler>
    {
        protected override OrderCommandsHandler CreateCommandsHandler()
        {
            return new OrderCommandsHandler(new InMemorySequenceProvider());
        }

        [Test]
        public void Order_should_throw_exñeption_on_item_add_with_negative_quantity()
        {

            Assert.Throws<InvalidQuantityException>(() =>
            {
                var cmd = new AddItemToOrderCommand(Aggregate.Id,
                                                    Guid.NewGuid(), 
                                                    -1, 
                                                    new Money(123));
                Execute(cmd);
            });
        }

        [Test]
        public void Order_should_throw_exñeption_on_item_add_with_negative_money()
        {
            Assert.Throws<InvalidMoneyException>(() =>
            {
                var cmd = new AddItemToOrderCommand(Aggregate.Id,
                                                    Guid.NewGuid(),
                                                    20,
                                                    new Money(-123));
                Execute(cmd);
            });
        }

        [OneTimeSetUp]
        public void Given_empty_order()
        {
            Init();
        }

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new OrderCreated(Guid.NewGuid(), 123,Guid.NewGuid());
        }
    }
}