using System;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.ProjectionBuilder
{
    
    public class SkuStock_added_without_sku_created_tests : SkuStockProjectionBuilderTests
    {
        [Fact]
        public async Task Given_sku_created_message_double_projected_When_project_Then_error_occures()
        {
            await ProjectionBuilder.Handle(new StockAdded(Guid.NewGuid(), 15, "test pack"))
                                   .ShouldThrow<SkuStockEntryNotFoundException>();
        }
    }
}