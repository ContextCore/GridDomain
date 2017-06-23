using System;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using Shop.Domain.Aggregates.SkuStockAggregate;
using Shop.Domain.Aggregates.SkuStockAggregate.Commands;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.Aggregate
{
    internal class SkuStock_negative_creation_tests
    {
        [Fact]
        public async Task When_creating_stock_with_negative_number_error_is_raised()
        {
            await AggregateScenario.New<SkuStock, SkuStockCommandsHandler>()
                                   .When(new CreateSkuStockCommand(Guid.NewGuid(),
                                                                   Guid.NewGuid(),
                                                                   -10,
                                                                   "test batch",
                                                                   TimeSpan.FromDays(1)))
                                   .RunAsync()
                                   .ShouldThrow<ArgumentException>();
        }
    }
}