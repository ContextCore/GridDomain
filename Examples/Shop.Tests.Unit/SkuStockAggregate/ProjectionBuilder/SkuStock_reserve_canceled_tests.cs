using System;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel.Context;
using Xunit;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    
    public class SkuStock_reserve_canceled_tests : SkuStockProjectionBuilderTests
    {
        private StockAdded _stockAddedEvent;
        private SkuStockCreated _stockCreatedEvent;
        private StockReserved _stockReservedEvent;
        private ReserveCanceled _reserveCanceledEvent;

        public SkuStock_reserve_canceled_tests()// Given_sku_created_and_stock_added_and_stock_reserved_messages_When_projected()
        {
            var stockId = Guid.NewGuid();

            _stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            _stockAddedEvent = new StockAdded(stockId, 15, "test pack");
            _stockReservedEvent = new StockReserved(stockId, Guid.NewGuid(), DateTime.Now.AddDays(1), 7);
            _reserveCanceledEvent = new ReserveCanceled(stockId, _stockReservedEvent.ReserveId);

            ProjectionBuilder.Handle(_stockCreatedEvent).Wait();
            ProjectionBuilder.Handle(_stockAddedEvent).Wait();
            ProjectionBuilder.Handle(_stockReservedEvent).Wait();
            ProjectionBuilder.Handle(_reserveCanceledEvent).Wait();
        }

       [Fact]
        public void Then_history_row_is_added()
        {
            using (var context = CreateContext())
            {
                Assert.NotNull(context.StockHistory.Find(_stockCreatedEvent.SourceId, (long) 4));
            }
        }

       [Fact]
        public void Then_history_row_is_filled()
        {
            using (var context = CreateContext())
            {
                //#1 is stock added history
                var history = context.StockHistory.Find(_stockCreatedEvent.SourceId, (long) 4);
                Assert.Equal(_stockCreatedEvent.Quantity + _stockAddedEvent.Quantity, history.NewAvailableQuantity);
                Assert.Equal(0, history.NewReservedQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity + _stockAddedEvent.Quantity, history.NewTotalQuantity);
                Assert.Equal(_stockReservedEvent.Quantity, history.OldReservedQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity + _stockAddedEvent.Quantity, history.OldTotalQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity + _stockAddedEvent.Quantity - _stockReservedEvent.Quantity,
                                history.OldAvailableQuantity);
                Assert.Equal(StockOperation.ReserveCanceled, history.Operation);
                Assert.Equal(_stockReservedEvent.Quantity, history.Quanity);
                Assert.Equal(_stockReservedEvent.SourceId, history.StockId);
                Assert.Equal(4, history.Number);
            }
        }

       [Fact]
        public void Then_reserve_row_is_removed()
        {
            using (var context = CreateContext())
            {
                Assert.Null(context.StockReserves.Find(_stockCreatedEvent.SourceId, _stockReservedEvent.ReserveId));
            }
        }

       [Fact]
        public void Then_sku_stock_row_is_modified()
        {
            using (var context = CreateContext())
            {
                var stock = context.SkuStocks.Find(_stockCreatedEvent.SourceId);
                Assert.Equal(_stockAddedEvent.Quantity + _stockCreatedEvent.Quantity, stock.AvailableQuantity);
                Assert.Equal(0, stock.CustomersReservationsTotal);
                Assert.Equal(_reserveCanceledEvent.CreatedTime, stock.LastModified);
                Assert.Equal(0, stock.ReservedQuantity);
                Assert.Equal(_stockAddedEvent.Quantity + _stockCreatedEvent.Quantity, stock.TotalQuantity);
            }
        }
    }
}