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
   
    public class Order_creates_from_event 
    {
        [Fact]
        public void Given_order_created()
        {
            var order = GridDomain.EventSourcing.Aggregate.Empty<Order>();
            var created = order.ApplyEvent(new OrderCreated(Guid.NewGuid(), 123, Guid.NewGuid()));
          // Order_items_are_empty()
            Assert.Empty(order.Items);
        //Order_receives_id()
            Assert.Equal(created.SourceId, order.Id);
        //Order_receives_number()
            Assert.Equal(created.Number, order.Number);
        //rder_status_is_created()
            Assert.Equal(OrderStatus.Created, order.Status);
       //User_receives_user()
            Assert.Equal(created.User, order.UserId);
        }
    }
}