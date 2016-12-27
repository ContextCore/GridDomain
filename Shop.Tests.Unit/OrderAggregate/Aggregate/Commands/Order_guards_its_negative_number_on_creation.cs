using System;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Commands;

namespace Shop.Tests.Unit.OrderAggregate.Aggregate.Commands
{
    [TestFixture]
    public class Order_guards_its_negative_number_on_creation : AggregateCommandsTest<Order,OrderCommandsHandler>
    {
        [Test]
        public void When_creating_order_with_negative_number_it_throws_exception()
        {
            Assert.Throws<NegativeOrderNumberException>(() =>
            {
                var cmd = new CreateOrderCommand(Guid.NewGuid(), -1, Guid.NewGuid());
                Execute(cmd);
            });
        }
    }
}