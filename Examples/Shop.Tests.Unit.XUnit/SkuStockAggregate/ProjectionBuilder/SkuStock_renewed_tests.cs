using System;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel.Context;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.ProjectionBuilder
{
   
    public class SkuStock_renewed_tests : SkuStockProjectionBuilderTests
    {
        private SkuStockCreated _stockCreatedEvent;
        private StockReserved _stockReservedEvent;
        private ReserveRenewed _stockReserveRenewedEvent;

        public SkuStock_renewed_tests()// Given_sku_created_and_reserved_and_reserve_expired_messages_When_projected()
        {
            var stockId = Guid.NewGuid();

            _stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            _stockReservedEvent = new StockReserved(stockId, Guid.NewGuid(), DateTime.Now.AddDays(1), 7);
            _stockReserveRenewedEvent = new ReserveRenewed(stockId, _stockReservedEvent.ReserveId);

            ProjectionBuilder.Handle(_stockCreatedEvent).Wait();
            ProjectionBuilder.Handle(_stockReservedEvent).Wait();
            ProjectionBuilder.Handle(_stockReserveRenewedEvent).Wait();
        }

       [Fact]
        public void Then_history_row_is_added()
        {
            using (var context = ContextFactory())
            {
                Assert.NotNull(context.StockHistory.Find(_stockCreatedEvent.SourceId, (long) 3));
            }
        }

       [Fact]
        public void Then_history_row_is_filled()
        {
            using (var context = ContextFactory())
            {
                //#1 is stock added history
                var history = context.StockHistory.Find(_stockCreatedEvent.SourceId, (long) 3);
                Assert.Equal(3, history.Number);

                Assert.Equal(_stockCreatedEvent.Quantity, history.NewAvailableQuantity);
                Assert.Equal(0, history.NewReservedQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity, history.NewTotalQuantity);
                Assert.Equal(_stockReservedEvent.Quantity, history.OldReservedQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity, history.OldTotalQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity - _stockReservedEvent.Quantity, history.OldAvailableQuantity);
                Assert.Equal(StockOperation.ReserveRenewed, history.Operation);
                Assert.Equal(_stockReservedEvent.Quantity, history.Quanity);
                Assert.Equal(_stockReservedEvent.SourceId, history.StockId);
            }
        }

       [Fact]
        public void Then_reserve_row_is_removed()
        {
            using (var context = ContextFactory())
            {
                Assert.Null(context.StockReserves.Find(_stockCreatedEvent.SourceId, _stockReservedEvent.ReserveId));
            }
        }

       [Fact]
        public void Then_sku_stock_row_is_modified()
        {
            using (var context = ContextFactory())
            {
                var stock = context.SkuStocks.Find(_stockCreatedEvent.SourceId);
                Assert.Equal(_stockCreatedEvent.Quantity, stock.AvailableQuantity);
                Assert.Equal(0, stock.CustomersReservationsTotal);
                Assert.Equal(_stockReserveRenewedEvent.CreatedTime, stock.LastModified);
                Assert.Equal(0, stock.ReservedQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity, stock.TotalQuantity);
            }
        }
    }
}