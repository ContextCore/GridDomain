using System.Linq;
using System.Threading.Tasks;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    
    public class SkuStock_added_two_times_tests : SkuStockProjectionBuilderTests
    {
        [Fact]
        public async Task Given_sku_created_message_double_projected_When_project_again_additioanal_transaction_occures()
        {
            var stockCreatedEvent = new Fixture().Create<SkuStockCreated>();
            var stockAddedEvent = new StockAdded(stockCreatedEvent.SourceId, 15, "test pack");

            await ProjectionBuilder.Handle(stockCreatedEvent);
            await ProjectionBuilder.Handle(stockAddedEvent);
            await ProjectionBuilder.Handle(stockAddedEvent);

            using (var context = CreateContext())
            {
                Assert.Equal(3, context.StockHistory.Count());
            }
        }
    }
}