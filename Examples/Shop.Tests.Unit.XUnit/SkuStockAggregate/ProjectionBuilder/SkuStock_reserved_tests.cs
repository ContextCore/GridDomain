using System;
using System.Linq;
using GridDomain.Logging;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel.Context;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.ProjectionBuilder
{
    
    public class SkuStock_reserved_tests : SkuStockProjectionBuilderTests
    {
        private StockAdded _stockAddedEvent;
        private SkuStockCreated _stockCreatedEvent;
        private StockReserved _stockReservedEvent;

        public SkuStock_reserved_tests()// Given_sku_created_and_stock_added_and_stock_reserved_messages_When_projected()
        {
            _stockCreatedEvent = new SkuStockCreated(Guid.NewGuid(), Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            _stockAddedEvent = new StockAdded(_stockCreatedEvent.SourceId, 15, "test pack");
            _stockReservedEvent = new StockReserved(_stockCreatedEvent.SourceId, Guid.NewGuid(), DateTime.Now.AddDays(1), 7);

            ProjectionBuilder.Handle(_stockCreatedEvent).Wait();
            ProjectionBuilder.Handle(_stockAddedEvent).Wait();
            ProjectionBuilder.Handle(_stockReservedEvent).Wait();
        }

       [Fact]
        public void Then_history_containes_rows_for_added_and_reserved_events()
        {
            using (var context = CreateContext())
            {
                Assert.Equal(3, context.StockHistory.Count());
            }
        }

       [Fact]
        public void Then_history_row_is_added()
        {
            using (var context = CreateContext())
            {
                var history = context.StockHistory.Find(_stockCreatedEvent.SourceId, (long) 3);
                if (history != null)
                    return;

                foreach (var hist in context.StockHistory)
                    Console.WriteLine(hist.ToPropsString());
                Assert.False(true,"Cannot find history");
            }
        }

       [Fact]
        public void Then_history_row_is_filled()
        {
            using (var context = CreateContext())
            {
                //#1 is stock added history
                var history = context.StockHistory.Find(_stockCreatedEvent.SourceId, (long) 3);
                Assert.Equal(_stockCreatedEvent.Quantity + _stockAddedEvent.Quantity - _stockReservedEvent.Quantity,
                                history.NewAvailableQuantity);
                Assert.Equal(_stockReservedEvent.Quantity, history.NewReservedQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity + _stockAddedEvent.Quantity, history.NewTotalQuantity);
                Assert.Equal(0, history.OldReservedQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity + _stockAddedEvent.Quantity, history.OldTotalQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity + _stockAddedEvent.Quantity, history.OldAvailableQuantity);
                Assert.Equal(StockOperation.Reserved, history.Operation);
                Assert.Equal(_stockReservedEvent.Quantity, history.Quanity);
                Assert.Equal(_stockReservedEvent.SourceId, history.StockId);
                Assert.Equal(3, history.Number);
            }
        }

       [Fact]
        public void Then_reserve_row_is_added()
        {
            using (var context = CreateContext())
            {
                Assert.NotNull(context.StockReserves.Find(_stockCreatedEvent.SourceId, _stockReservedEvent.ReserveId));
            }
        }

       [Fact]
        public void Then_reserve_row_is_filled()
        {
            using (var context = CreateContext())
            {
                var reserve = context.StockReserves.Find(_stockCreatedEvent.SourceId, _stockReservedEvent.ReserveId);

                Assert.Equal(_stockReservedEvent.ReserveId, reserve.CustomerId);
                Assert.Equal(_stockCreatedEvent.SkuId, reserve.SkuId);
                Assert.Equal(_stockReservedEvent.SourceId, reserve.StockId);
                Assert.Equal(_stockReservedEvent.CreatedTime, reserve.Created);
                Assert.Equal(_stockReservedEvent.ExpirationDate, reserve.ExpirationDate);
                Assert.Equal(_stockReservedEvent.Quantity, reserve.Quantity);
            }
        }

       [Fact]
        public void Then_sku_stock_row_is_modified()
        {
            using (var context = CreateContext())
            {
                var stock = context.SkuStocks.Find(_stockCreatedEvent.SourceId);
                Assert.Equal(_stockAddedEvent.Quantity + _stockCreatedEvent.Quantity - _stockReservedEvent.Quantity,
                                stock.AvailableQuantity);
                Assert.Equal(1, stock.CustomersReservationsTotal);
                Assert.Equal(_stockReservedEvent.CreatedTime, stock.LastModified);
                Assert.Equal(_stockReservedEvent.Quantity, stock.ReservedQuantity);
                Assert.Equal(_stockAddedEvent.Quantity + _stockCreatedEvent.Quantity, stock.TotalQuantity);
            }
        }
    }
}