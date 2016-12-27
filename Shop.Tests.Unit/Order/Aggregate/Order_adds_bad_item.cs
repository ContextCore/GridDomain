using System;
using GridDomain.Tests.Framework;
using NMoneys;
using NUnit.Framework;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Events;

namespace Shop.Tests.Unit.Order
{
    [TestFixture]
    public class Order_adds_bad_item : HydrationSpecification<Domain.Aggregates.OrderAggregate.Order>
    {
        [Test]
        public void Order_should_throw_exñeption_on_item_add_with_negative_quantity()
        {
            Assert.Throws<InvalidQuantityException>(() =>
                Aggregate.AddItem(Guid.NewGuid(), -1, new Money(123)));
        }

        [Test]
        public void Order_should_throw_exñeption_on_item_add_with_negative_money()
        {
            Assert.Throws<InvalidMoneyException>(() =>
                Aggregate.AddItem(Guid.NewGuid(), 20, new Money(-100)));
        }

        [OneTimeSetUp]
        public void Given_empty_order()
        {
            var created = new OrderCreated(Guid.NewGuid(), 123,Guid.NewGuid());
        }
    }
}