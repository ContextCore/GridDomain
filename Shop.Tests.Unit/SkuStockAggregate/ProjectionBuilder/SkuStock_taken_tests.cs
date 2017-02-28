using System;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    [TestFixture]
    public class SkuStock_taken_tests : SkuStockProjectionBuilderTests
    {
        private SkuStockCreated _stockCreatedEvent;
        private StockTaken _stockTakenEvent;

        [OneTimeSetUp]
        public void Given_sku_created_and_taken_messages_When_projected()
        {
            var stockId = Guid.NewGuid();

            _stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 5, TimeSpan.FromDays(2));
            _stockTakenEvent = new StockTaken(stockId, 3);

            ProjectionBuilder.Handle(_stockCreatedEvent);
            ProjectionBuilder.Handle(_stockTakenEvent);
        }

        [Test]
        public void Then_history_row_is_added()
        {
            using (var context = ContextFactory()) {
                Assert.NotNull(context.StockHistory.Find(_stockCreatedEvent.SourceId, (long) 2));
            }
        }

        [Test]
        public void Then_history_row_is_filled()
        {
            using (var context = ContextFactory())
            {
                //#1 is stock added history
                var history = context.StockHistory.Find(_stockCreatedEvent.SourceId, (long) 2);
                Assert.AreEqual(2, history.Number);

                Assert.AreEqual(_stockCreatedEvent.Quantity - _stockTakenEvent.Quantity, history.NewAvailableQuantity);
                Assert.AreEqual(0, history.NewReservedQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity - _stockTakenEvent.Quantity, history.NewTotalQuantity);
                Assert.AreEqual(0, history.OldReservedQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity, history.OldTotalQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity, history.OldAvailableQuantity);
                Assert.AreEqual(StockOperation.Taken, history.Operation);
                Assert.AreEqual(_stockTakenEvent.Quantity, history.Quanity);
                Assert.AreEqual(_stockTakenEvent.SourceId, history.StockId);
            }
        }

        [Test]
        public void Then_sku_stock_row_is_modified()
        {
            using (var context = ContextFactory())
            {
                var stock = context.SkuStocks.Find(_stockCreatedEvent.SourceId);
                Assert.AreEqual(_stockCreatedEvent.Quantity - _stockTakenEvent.Quantity, stock.AvailableQuantity);
                Assert.AreEqual(0, stock.CustomersReservationsTotal);
                Assert.AreEqual(_stockTakenEvent.CreatedTime, stock.LastModified);
                Assert.AreEqual(0, stock.ReservedQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity - _stockTakenEvent.Quantity, stock.TotalQuantity);
            }
        }
    }
}