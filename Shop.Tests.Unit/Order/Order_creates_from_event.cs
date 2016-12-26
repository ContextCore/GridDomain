using System;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.UserAggregate;

namespace Shop.Tests.Unit.Order
{
    [TestFixture]
    public class Order_creates_from_event : HydrationSpecification<Domain.Aggregates.OrderAggregate.Order>
    {
        private OrderCreated _createdEvent;

        [OneTimeSetUp]
        public void Given_order_created()
        {
            _createdEvent = new OrderCreated(Aggregate.Id, 123, Guid.NewGuid());
            Aggregate.ApplyEvents(_createdEvent);
        }

        [Test]
        public void Order_receives_id()
        {
            Assert.AreEqual(_createdEvent.Id, Aggregate.Id);
        }
        [Test]
        public void Order_status_is_created()
        {
            Assert.AreEqual(OrderStatus.Created, Aggregate.Status);
        }
        [Test]
        public void Order_receives_number()
        {
            Assert.AreEqual(_createdEvent.Number, Aggregate.Number);
        }

        [Test]
        public void Order_items_are_empty()
        {
            CollectionAssert.IsEmpty(Aggregate.Items);
        }

        [Test]
        public void User_receives_user()
        {
            Assert.AreEqual(_createdEvent.User, Aggregate.UserId);
        }

    }
}