using System;
using System.Linq;
using System.Threading.Tasks;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.ProjectionBuilder
{
   
    public class SkuStock_taken_errors_test : SkuStockProjectionBuilderTests
    {
       [Fact]
        public async Task Given_no_stock_When_stock_taken_projected_Then_error_occurs()
        {
            var reserveTaken = new StockReserveTaken(Guid.NewGuid(), Guid.NewGuid());
            await Assert.ThrowsAsync<SkuStockEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveTaken));
        }

       [Fact]
        public async Task Given_sku_created_and_taken_messages_When_projected_Then_another_history_is_added()
        {
            var stockId = Guid.NewGuid();

            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 7, TimeSpan.FromDays(2));
            var stockTaken = new StockTaken(stockId, 3);

            await ProjectionBuilder.Handle(stockCreatedEvent);
            await ProjectionBuilder.Handle(stockTaken);
            await ProjectionBuilder.Handle(stockTaken);

            Assert.Equal(3, ContextFactory().StockHistory.Count());
            Assert.Equal(1, ContextFactory().SkuStocks.Find(stockId).AvailableQuantity);
        }
    }
}