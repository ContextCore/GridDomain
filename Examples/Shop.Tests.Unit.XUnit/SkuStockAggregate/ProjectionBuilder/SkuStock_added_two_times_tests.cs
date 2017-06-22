using System.Linq;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.ProjectionBuilder
{
   
    public class SkuStock_added_two_times_tests : SkuStockProjectionBuilderTests
    {
        private StockAdded _stockAddedEvent;
        private SkuStockCreated _stockCreatedEvent;

        public SkuStock_added_two_times_tests()// Given_sku_created_message_double_projected()
        {
            _stockCreatedEvent = new Fixture().Create<SkuStockCreated>();
            _stockAddedEvent = new StockAdded(_stockCreatedEvent.SourceId, 15, "test pack");

            ProjectionBuilder.Handle(_stockCreatedEvent).Wait();
            ProjectionBuilder.Handle(_stockAddedEvent).Wait();
            ProjectionBuilder.Handle(_stockAddedEvent).Wait();
        }

       [Fact]
        public void When_project_again_additioanal_transaction_occures()
        {
            using (var context = ContextFactory())
            {
                Assert.Equal(3, context.StockHistory.Count());
            }
        }
    }
}