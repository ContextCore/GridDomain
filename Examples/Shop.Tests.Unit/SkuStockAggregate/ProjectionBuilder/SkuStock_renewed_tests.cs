using System;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    [TestFixture]
    public class SkuStock_renewed_tests : SkuStockProjectionBuilderTests
    {
        private SkuStockCreated _stockCreatedEvent;
        private StockReserved _stockReservedEvent;
        private ReserveRenewed _stockReserveRenewedEvent;

        [OneTimeSetUp]
        public void Given_sku_created_and_reserved_and_reserve_expired_messages_When_projected()
        {
            var stockId = Guid.NewGuid();

            _stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            _stockReservedEvent = new StockReserved(stockId, Guid.NewGuid(), DateTime.Now.AddDays(1), 7);
            _stockReserveRenewedEvent = new ReserveRenewed(stockId, _stockReservedEvent.ReserveId);

            ProjectionBuilder.Handle(_stockCreatedEvent);
            ProjectionBuilder.Handle(_stockReservedEvent);
            ProjectionBuilder.Handle(_stockReserveRenewedEvent);
        }

        [Test]
        public void Then_history_row_is_added()
        {
            using (var context = ContextFactory())
            {
                Assert.NotNull(context.StockHistory.Find(_stockCreatedEvent.SourceId, (long) 3));
            }
        }

        [Test]
        public void Then_history_row_is_filled()
        {
            using (var context = ContextFactory())
            {
                //#1 is stock added history
                var history = context.StockHistory.Find(_stockCreatedEvent.SourceId, (long) 3);
                Assert.AreEqual(3, history.Number);

                Assert.AreEqual(_stockCreatedEvent.Quantity, history.NewAvailableQuantity);
                Assert.AreEqual(0, history.NewReservedQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity, history.NewTotalQuantity);
                Assert.AreEqual(_stockReservedEvent.Quantity, history.OldReservedQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity, history.OldTotalQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity - _stockReservedEvent.Quantity, history.OldAvailableQuantity);
                Assert.AreEqual(StockOperation.ReserveRenewed, history.Operation);
                Assert.AreEqual(_stockReservedEvent.Quantity, history.Quanity);
                Assert.AreEqual(_stockReservedEvent.SourceId, history.StockId);
            }
        }

        [Test]
        public void Then_reserve_row_is_removed()
        {
            using (var context = ContextFactory())
            {
                Assert.Null(context.StockReserves.Find(_stockCreatedEvent.SourceId, _stockReservedEvent.ReserveId));
            }
        }

        [Test]
        public void Then_sku_stock_row_is_modified()
        {
            using (var context = ContextFactory())
            {
                var stock = context.SkuStocks.Find(_stockCreatedEvent.SourceId);
                Assert.AreEqual(_stockCreatedEvent.Quantity, stock.AvailableQuantity);
                Assert.AreEqual(0, stock.CustomersReservationsTotal);
                Assert.AreEqual(_stockReserveRenewedEvent.CreatedTime, stock.LastModified);
                Assert.AreEqual(0, stock.ReservedQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity, stock.TotalQuantity);
            }
        }
    }
}