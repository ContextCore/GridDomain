using System;
using System.Threading.Tasks;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.ProjectionBuilder
{
   
    public class SkuStock_double_reserved_tests : SkuStockProjectionBuilderTests
    {
        private StockAdded _stockAddedEvent;
        private SkuStockCreated _stockCreatedEvent;
        private StockReserved _stockReservedEvent;

        public SkuStock_double_reserved_tests()// Given_sku_created_and_stock_added_and_stock_reserved_messages_When_projected()
        {
            _stockCreatedEvent = new SkuStockCreated(Guid.NewGuid(), Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            _stockAddedEvent = new StockAdded(_stockCreatedEvent.SourceId, 15, "test pack");
            _stockReservedEvent = new StockReserved(_stockCreatedEvent.SourceId, Guid.NewGuid(), DateTime.Now.AddDays(1), 7);

            ProjectionBuilder.Handle(_stockCreatedEvent).Wait();
            ProjectionBuilder.Handle(_stockAddedEvent).Wait();
            ProjectionBuilder.Handle(_stockReservedEvent).Wait();
        }

       [Fact]
        public async Task When_project_reserve_again_error_is_occured()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => ProjectionBuilder.Handle(_stockReservedEvent));
        }
    }
}