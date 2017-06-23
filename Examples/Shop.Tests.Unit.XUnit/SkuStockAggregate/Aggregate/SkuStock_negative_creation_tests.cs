using System;
using GridDomain.Tests.Common;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.Aggregate
{
   
    internal class SkuStock_negative_creation_tests : AggregateCommandsTest<SkuStock, SkuStockCommandsHandler>
    {
       [Fact]
        public void When_creating_stock_with_negative_number_error_is_raised()
        {
            Init();
            var command = new CreateSkuStockCommand(Aggregate.Id, Guid.NewGuid(), -10, "test batch", TimeSpan.FromDays(1));
            Assert.ThrowsAsync<ArgumentException>(async () => await Execute(command));
        }
    }
}