using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.UserAggregate;

namespace Shop.Tests.Unit.OrderAggregate.Aggregate.Hydration
{
    [TestFixture]
    public class Order_creates_from_event : AggregateTest<Domain.Aggregates.OrderAggregate.Order>
    {
        private OrderCreated _createdEvent;

        [OneTimeSetUp]
        public void Given_order_created()
        {
            Init();
        }

        protected override IEnumerable<DomainEvent> Given()
        {
           yield return
             _createdEvent = new OrderCreated(Aggregate.Id, 123, Guid.NewGuid());
        }

        [Test]
        public void Order_receives_id()
        {
            Assert.AreEqual(_createdEvent.SourceId, Aggregate.Id);
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