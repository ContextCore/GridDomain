using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.EventSourcing;
using GridDomain.Tests.Common;
using NMoneys;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.OrderAggregate.Aggregate.Hydration
{
   
    public class Order_when_adding_correct_item_tests : AggregateTest<Order>
    {
        public Order_when_adding_correct_item_tests () //When_adding_new_item()
        {
            Init();

            _initialSum = Aggregate.TotalPrice;
            _initialItemsCount = Aggregate.Items.Count;

            _itemAddedEventB = new ItemAdded(Aggregate.Id, Guid.NewGuid(), 1, new Money(50), 2);
            Aggregate.ApplyEvents(_itemAddedEventB);
        }

        protected override IEnumerable<DomainEvent> Given()
        {
            yield return new OrderCreated(Aggregate.Id, 1, Guid.NewGuid());
            yield return new ItemAdded(Aggregate.Id, Guid.NewGuid(), 2, new Money(100), 1);
        }

        private Money _initialSum;
        private ItemAdded _itemAddedEventB;
        private int _initialItemsCount;

       [Fact]
        public void Order_items_contains_new_one_as_separate_item()
        {
            Assert.True(
                        Aggregate.Items.Any(
                                            i =>
                                                i.Quantity == _itemAddedEventB.Quantity && i.Sku == _itemAddedEventB.Sku
                                                && i.TotalPrice == _itemAddedEventB.TotalPrice));
        }

       [Fact]
        public void Order_items_count_is_increased_by_one()
        {
            Assert.Equal(_initialItemsCount + 1, Aggregate.Items.Count);
        }

       [Fact]
        public void Order_total_sum_is_increased_by_new_item_total()
        {
            Assert.Equal(_itemAddedEventB.TotalPrice, Aggregate.TotalPrice - _initialSum);
        }
    }
}