using System;
using System.Threading.Tasks;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.ProjectionBuilder
{
   
    public class SkuStock_added_without_sku_created_tests : SkuStockProjectionBuilderTests
    {
        private StockAdded _stockAddedEvent;

        public SkuStock_added_without_sku_created_tests()// Given_sku_created_message_double_projected()
        {
            _stockAddedEvent = new StockAdded(Guid.NewGuid(), 15, "test pack");
        }

       [Fact]
        public async Task When_project_Then_error_occures()
        {
            await Assert.ThrowsAsync<SkuStockEntryNotFoundException>(() => ProjectionBuilder.Handle(_stockAddedEvent));
        }
    }
}