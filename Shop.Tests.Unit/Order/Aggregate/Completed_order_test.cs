using System;
using GridDomain.Tests.Framework;
using NMoneys;
using NUnit.Framework;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.UserAggregate;

namespace Shop.Tests.Unit.Order
{
    [TestFixture]
    public class Completed_order_test : HydrationSpecification<Domain.Aggregates.OrderAggregate.Order>
    {
        [Test]
        public void When_order_completes_it_chages_status_to_paid()
        {
            Assert.AreEqual(OrderStatus.Paid, Aggregate.Status);
        }

        [Test]
        public void When_completed_order_try_complete_it_throws_exeption()
        {
            Assert.Throws<CannotCompleteAlreadyClosedOrderException>(() =>
                Aggregate.Complete());
        }
        [Test]
        public void When_completed_order_try_add_items_it_throws_exeption()
        {
            Assert.Throws<CantAddItemsToClosedOrder>(() =>
                Aggregate.AddItem(Guid.NewGuid(),123,new Money(1)));
        }

        [SetUp]
        public void Given_completed_order()
        {
            var evt = new OrderCreated(Aggregate.Id, 123, Guid.NewGuid());
            Aggregate.ApplyEvents(evt);

            Aggregate.Complete();
        }
    }
}