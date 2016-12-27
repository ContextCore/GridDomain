using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.EventSourcing;
using GridDomain.Tests.Framework;
using NMoneys;
using NUnit.Framework;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Events;

namespace Shop.Tests.Unit.OrderAggregate.Aggregate.Hydration
{
    [TestFixture]
    public class Order_when_adding_correct_item_tests : AggregateTest<Order>
    {
        [Test]
        public void Order_items_contains_new_one_as_separate_item()
        {
            Assert.True(Aggregate.Items.Any(i =>
                                 i.Quantity == _itemAddedEventB.Quantity && 
                                 i.Sku == _itemAddedEventB.Sku &&
                                 i.TotalPrice == _itemAddedEventB.TotalPrice
                                 ));
        }

        [Test]
        public void Order_total_sum_is_increased_by_new_item_total()
        {
            Assert.AreEqual(_itemAddedEventB.TotalPrice, Aggregate.TotalPrice - _initialSum);
        }

        [Test]
        public void Order_items_count_is_increased_by_one()
        {
            Assert.AreEqual(_initialItemsCount + 1, Aggregate.Items.Count);
        }

        [OneTimeSetUp]
        public void When_adding_new_item()
        {
            Init();

            _initialSum = Aggregate.TotalPrice;
            _initialItemsCount = Aggregate.Items.Count;

            _itemAddedEventB = new ItemAdded(Aggregate.Id, Guid.NewGuid(), 1, new Money(50),2);
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
    }
}