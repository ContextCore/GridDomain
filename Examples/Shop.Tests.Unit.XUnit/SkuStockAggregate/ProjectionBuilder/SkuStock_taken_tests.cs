using System;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel.Context;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.ProjectionBuilder
{
   
    public class SkuStock_taken_tests : SkuStockProjectionBuilderTests
    {
        private SkuStockCreated _stockCreatedEvent;
        private StockTaken _stockTakenEvent;

        public SkuStock_taken_tests()// Given_sku_created_and_taken_messages_When_projected()
        {
            var stockId = Guid.NewGuid();

            _stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 5, TimeSpan.FromDays(2));
            _stockTakenEvent = new StockTaken(stockId, 3);

            ProjectionBuilder.Handle(_stockCreatedEvent).Wait();
            ProjectionBuilder.Handle(_stockTakenEvent).Wait();
        }

       [Fact]
        public void Then_history_row_is_added()
        {
            using (var context = ContextFactory())
            {
                Assert.NotNull(context.StockHistory.Find(_stockCreatedEvent.SourceId, (long) 2));
            }
        }

       [Fact]
        public void Then_history_row_is_filled()
        {
            using (var context = ContextFactory())
            {
                //#1 is stock added history
                var history = context.StockHistory.Find(_stockCreatedEvent.SourceId, (long) 2);
                Assert.Equal(2, history.Number);

                Assert.Equal(_stockCreatedEvent.Quantity - _stockTakenEvent.Quantity, history.NewAvailableQuantity);
                Assert.Equal(0, history.NewReservedQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity - _stockTakenEvent.Quantity, history.NewTotalQuantity);
                Assert.Equal(0, history.OldReservedQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity, history.OldTotalQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity, history.OldAvailableQuantity);
                Assert.Equal(StockOperation.Taken, history.Operation);
                Assert.Equal(_stockTakenEvent.Quantity, history.Quanity);
                Assert.Equal(_stockTakenEvent.SourceId, history.StockId);
            }
        }

       [Fact]
        public void Then_sku_stock_row_is_modified()
        {
            using (var context = ContextFactory())
            {
                var stock = context.SkuStocks.Find(_stockCreatedEvent.SourceId);
                Assert.Equal(_stockCreatedEvent.Quantity - _stockTakenEvent.Quantity, stock.AvailableQuantity);
                Assert.Equal(0, stock.CustomersReservationsTotal);
                Assert.Equal(_stockTakenEvent.CreatedTime, stock.LastModified);
                Assert.Equal(0, stock.ReservedQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity - _stockTakenEvent.Quantity, stock.TotalQuantity);
            }
        }
    }
}