using System;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.ProjectionBuilder
{
    
    public class SkuStock_reserved_wihout_sku_entry_tests : SkuStockProjectionBuilderTests
    {
        [Fact]
        public async Task Given_stock_reserved_message_without_stock_created_When_projected_When_projected_then_error_occures()
        {
            await ProjectionBuilder.Handle(new StockReserved(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now.AddDays(1), 7))
                                   .ShouldThrow<SkuStockEntryNotFoundException>();
        }
    }
}