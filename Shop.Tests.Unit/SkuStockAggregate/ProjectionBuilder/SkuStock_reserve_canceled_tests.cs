using System;
using System.Linq;
using GridDomain.Logging;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    [TestFixture]
    public class SkuStock_reserve_canceled_tests : SkuStockProjectionBuilderTests
    {
        private StockAdded      _stockAddedEvent;
        private SkuStockCreated _stockCreatedEvent;
        private StockReserved   _stockReservedEvent;
        private ReserveCanceled  _reserveCanceledEvent;

        [OneTimeSetUp]
        public void Given_sku_created_and_stock_added_and_stock_reserved_messages_When_projected()
        {
            var stockId = Guid.NewGuid();

            _stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            _stockAddedEvent = new StockAdded(stockId, 15, "test pack");
            _stockReservedEvent = new StockReserved(stockId, Guid.NewGuid(), DateTime.Now.AddDays(1),7);
            _reserveCanceledEvent = new ReserveCanceled(stockId, _stockReservedEvent.ReserveId);

            ProjectionBuilder.Handle(_stockCreatedEvent);
            ProjectionBuilder.Handle(_stockAddedEvent);
            ProjectionBuilder.Handle(_stockReservedEvent);
            ProjectionBuilder.Handle(_reserveCanceledEvent);
        }

        [Test]
        public void Then_history_row_is_added()
        {
            using (var context = ContextFactory())
            {
                Assert.NotNull(context.StockHistory.Find(_stockCreatedEvent.SourceId, (long)4));
            }
        }

        [Test]
        public void Then_history_row_is_filled()
        {
            using (var context = ContextFactory())
            {
                //#1 is stock added history
                var history = context.StockHistory.Find(_stockCreatedEvent.SourceId, (long)4);
                Assert.AreEqual(_stockCreatedEvent.Quantity +
                                _stockAddedEvent.Quantity, history.NewAvailableQuantity);
                Assert.AreEqual(0, history.NewReservedQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity +
                                _stockAddedEvent.Quantity, history.NewTotalQuantity);
                Assert.AreEqual(_stockReservedEvent.Quantity, history.OldReservedQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity +
                                _stockAddedEvent.Quantity, history.OldTotalQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity +
                                _stockAddedEvent.Quantity - 
                                _stockReservedEvent.Quantity, history.OldAvailableQuantity);
                Assert.AreEqual(StockOperation.ReserveCanceled, history.Operation);
                Assert.AreEqual(_stockReservedEvent.Quantity, history.Quanity);
                Assert.AreEqual(_stockReservedEvent.SourceId, history.StockId);
                Assert.AreEqual(4, history.Number);
            }
        }

        [Test]
        public void Then_reserve_row_is_removed()
        {
            using (var context = ContextFactory())
                Assert.Null(context.StockReserves.Find(_stockCreatedEvent.SourceId, _stockReservedEvent.ReserveId));

        }


        [Test]
        public void Then_sku_stock_row_is_modified()
        {
            using (var context = ContextFactory())
            {
                var stock = context.SkuStocks.Find(_stockCreatedEvent.SourceId);
                Assert.AreEqual(_stockAddedEvent.Quantity +
                                _stockCreatedEvent.Quantity, stock.AvailableQuantity);
                Assert.AreEqual(0, stock.CustomersReservationsTotal);
                Assert.AreEqual(_reserveCanceledEvent.CreatedTime, stock.LastModified);
                Assert.AreEqual(0, stock.ReservedQuantity);
                Assert.AreEqual(_stockAddedEvent.Quantity +
                                _stockCreatedEvent.Quantity, stock.TotalQuantity);

            }
        }
    }
}