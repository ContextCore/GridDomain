using System;
using System.Threading.Tasks;
using GridDomain.Tests.Framework;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;

namespace Shop.Tests.Unit.SkuStockAggregate.Aggregate
{
    [TestFixture]
    internal class SkuStock_negative_creation_tests : AggregateCommandsTest<SkuStock, SkuStockCommandsHandler>
    {
        [Test]
        public async Task When_creating_stock_with_negative_number_error_is_raised()
        {
            Init();
            var command = new CreateSkuStockCommand(Aggregate.Id, Guid.NewGuid(), -10, "test batch", TimeSpan.FromDays(1));
            Assert.ThrowsAsync<ArgumentException>(async () => await Execute(command));
        }
    }
}