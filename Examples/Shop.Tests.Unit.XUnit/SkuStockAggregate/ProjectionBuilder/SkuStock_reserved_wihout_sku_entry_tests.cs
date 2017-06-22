using System;
using System.Threading.Tasks;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.ProjectionBuilder
{
   
    public class SkuStock_reserved_wihout_sku_entry_tests : SkuStockProjectionBuilderTests
    {
        private StockReserved _stockReservedEvent;

        public SkuStock_reserved_wihout_sku_entry_tests()// Given_stock_reserved_message_without_stock_created_When_projected()
        {
            _stockReservedEvent = new StockReserved(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now.AddDays(1), 7);
        }

       [Fact]
        public async Task When_projected_then_error_occures()
        {
            await Assert.ThrowsAsync<SkuStockEntryNotFoundException>(() => ProjectionBuilder.Handle(_stockReservedEvent));
        }
    }
}