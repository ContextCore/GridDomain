using System;
using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.Tests.Common;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.Domain.Aggregates.UserAggregate;
using Xunit;

namespace Shop.Tests.Unit.XUnit.OrderAggregate.Aggregate.Hydration
{
   
    public class Order_creates_from_event : AggregateTest<Order>
    {
        private OrderCreated _createdEvent;

        public Order_creates_from_event()// Given_order_created()
        {
            Init();
        }

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return _createdEvent = new OrderCreated(Aggregate.Id, 123, Guid.NewGuid());
        }

       [Fact]
        public void Order_items_are_empty()
        {
             Assert.Empty(Aggregate.Items);
        }

       [Fact]
        public void Order_receives_id()
        {
            Assert.Equal(_createdEvent.SourceId, Aggregate.Id);
        }

       [Fact]
        public void Order_receives_number()
        {
            Assert.Equal(_createdEvent.Number, Aggregate.Number);
        }

       [Fact]
        public void Order_status_is_created()
        {
            Assert.Equal(OrderStatus.Created, Aggregate.Status);
        }

       [Fact]
        public void User_receives_user()
        {
            Assert.Equal(_createdEvent.User, Aggregate.UserId);
        }
    }
}