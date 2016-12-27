using System;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Shop.Domain.Aggregates.OrderAggregate;

namespace Shop.Tests.Unit.Order
{
    [TestFixture]
    public class Order_guards_its_negative_number : HydrationSpecification<Domain.Aggregates.OrderAggregate.Order>
    {
        [Test]
        public void When_creating_order_with_negative_number_it_throws_exception()
        {
            Assert.Throws<NegativeOrderNumberException>(() => new Domain.Aggregates.OrderAggregate.Order(Guid.NewGuid(), -1, Guid.NewGuid()));
        }
    }
}