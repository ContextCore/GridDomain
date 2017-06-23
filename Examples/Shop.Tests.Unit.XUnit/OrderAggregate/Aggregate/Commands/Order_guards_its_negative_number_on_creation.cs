using System;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using Moq;
using Shop.Domain.Aggregates.OrderAggregate;
using Shop.Domain.Aggregates.OrderAggregate.Commands;
using Shop.Domain.Aggregates.OrderAggregate.Exceptions;
using Shop.Infrastructure;
using Xunit;

namespace Shop.Tests.Unit.XUnit.OrderAggregate.Aggregate.Commands
{
    public class Order_guards_its_negative_number_on_creation
    {
        [Fact]
        public async Task When_creating_order_with_negative_number_it_throws_exception()
        {
            var sequenceNumberMock = new Mock<ISequenceProvider>();
            sequenceNumberMock.Setup(p => p.GetNext(It.IsAny<string>())).Returns(-1);

            await AggregateScenario.New(new OrderCommandsHandler(sequenceNumberMock.Object))
                                   .When(new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid()))
                                   .RunAsync()
                                   .ShouldThrow<NegativeOrderNumberException>();
        }
    }
}