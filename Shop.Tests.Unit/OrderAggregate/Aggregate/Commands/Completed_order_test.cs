using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NMoneys;
using NUnit.Framework;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Commands;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.UserAggregate;
using Shop.Infrastructure;

namespace Shop.Tests.Unit.OrderAggregate.Aggregate.Commands
{
    [TestFixture]
    public class Completed_order_test : AggregateCommandsTest<Order,OrderCommandsHandler>
    {
        protected override OrderCommandsHandler CreateCommandsHandler()
        {
            return new OrderCommandsHandler(new InMemorySequenceProvider());
        }

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new OrderCreated(Aggregate.Id, 123, Guid.NewGuid());
            yield return new OrderCompleted(Aggregate.Id, OrderStatus.Paid);
        }

        [SetUp]
        public void Given_completed_order()
        {
            Init();
        }

        [Test]
        public void When_order_completes_it_chages_status_to_paid()
        {
            Assert.AreEqual(OrderStatus.Paid, Aggregate.Status);
        }

        [Test]
        public void When_completed_order_try_complete_it_throws_exeption()
        {
            Assert.Throws<CannotCompleteAlreadyClosedOrderException>(() =>
                Execute(new CompleteOrderCommand(Aggregate.Id)));
        }

        [Test]
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
        
   
    }
}