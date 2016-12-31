using System;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate;

namespace Shop.Tests.Unit.SkuStockAggregate.Aggregate
{
    [TestFixture]
    class SkuStock_negative_creation_tests : AggregateCommandsTest<SkuStock, SkuStockCommandsHandler>
    {
        [Test]
        public void When_creating_stock_with_negative_number_error_is_raised()
        {
            Init();
            var command = new CreateSkuStockCommand(Aggregate.Id, Guid.NewGuid(), -10, "test batch");
            Assert.Throws<ArgumentException>( () => Execute(command));
        }
    }
}