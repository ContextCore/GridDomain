using System;
using GridDomain.Tests.Framework;
using Moq;
using NUnit.Framework;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Commands;
using Shop.Domain.Aggregates.OrderAggregate.Exceptions;
using Shop.Infrastructure;

namespace Shop.Tests.Unit.OrderAggregate.Aggregate.Commands
{
    [TestFixture]
    public class Order_guards_its_negative_number_on_creation : AggregateCommandsTest<Order, OrderCommandsHandler>
    {
        protected override OrderCommandsHandler CreateCommandsHandler()
        {
            var sequenceNumberMock = new Mock<ISequenceProvider>();
            sequenceNumberMock.Setup(p => p.GetNext(It.IsAny<string>())).Returns(-1);

            return new OrderCommandsHandler(sequenceNumberMock.Object);
        }

        [Test]
        public void When_creating_order_with_negative_number_it_throws_exception()
        {
            Assert.ThrowsAsync<NegativeOrderNumberException>(async () =>
                                                             {
                                                                 var cmd = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid());
                                                                 await Execute(cmd);
                                                             });
        }
    }
}