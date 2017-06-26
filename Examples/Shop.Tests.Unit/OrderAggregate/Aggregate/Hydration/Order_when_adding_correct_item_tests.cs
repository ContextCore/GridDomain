using System;
using System.Linq;
using GridDomain.Tests.Common;
using NMoneys;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.OrderAggregate.Aggregate.Hydration
{
   
    public class Order_when_adding_correct_item_tests
    {
        [Fact]
        public void When_adding_new_item()
        {
            var aggregate = GridDomain.EventSourcing.Aggregate.Empty<Order>();
            aggregate.ApplyEvents(new OrderCreated(aggregate.Id, 1, Guid.NewGuid()),
                                  new ItemAdded(aggregate.Id, Guid.NewGuid(), 2, new Money(100), 1));

            var initialSum = aggregate.TotalPrice;
            var initialItemsCount = aggregate.Items.Count;

            var itemAddedEventB = aggregate.ApplyEvent(new ItemAdded(aggregate.Id, Guid.NewGuid(), 1, new Money(50), 2));
     //Order_items_contains_new_one_as_separate_item()
            Assert.True(aggregate.Items.Any(i =>
                                                i.Quantity == itemAddedEventB.Quantity && i.Sku == itemAddedEventB.Sku
                                                && i.TotalPrice == itemAddedEventB.TotalPrice));
        //Order_items_count_is_increased_by_one()
            Assert.Equal(initialItemsCount + 1, aggregate.Items.Count);
       //Order_total_sum_is_increased_by_new_item_total()
            Assert.Equal(itemAddedEventB.TotalPrice, aggregate.TotalPrice - initialSum);
        }
    }
}