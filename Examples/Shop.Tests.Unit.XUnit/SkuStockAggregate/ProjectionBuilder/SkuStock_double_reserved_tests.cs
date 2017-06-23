using System;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.ProjectionBuilder
{
    public class SkuStock_double_reserved_tests : SkuStockProjectionBuilderTests
    {
        [Fact]
        public async Task Given_sku_created_and_stock_added_and_stock_reserved_messages_When_projected()
        {
            var sourceId = Guid.NewGuid();

            await ProjectionBuilder.Handle(new SkuStockCreated(sourceId, Guid.NewGuid(), 1, TimeSpan.FromDays(2)));
            await ProjectionBuilder.Handle(new StockAdded(sourceId, 15, "test pack"));
            await ProjectionBuilder.Handle(new StockReserved(sourceId, Guid.NewGuid(), DateTime.Now.AddDays(1), 7));
            //When_project_reserve_again_error_is_occured()
            await ProjectionBuilder.Handle(new StockReserved(sourceId, Guid.NewGuid(), DateTime.Now.AddDays(1), 7))
                                   .ShouldThrow<ArgumentException>();
        }
    }
}