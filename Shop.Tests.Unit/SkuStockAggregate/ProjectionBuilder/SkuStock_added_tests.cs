using System;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    [TestFixture]
    public class SkuStock_added_tests : SkuStockProjectionBuilderTests
    {
        private StockAdded _stockAddedEvent;
        private SkuStockCreated _stockCreatedEvent;

        [OneTimeSetUp]
        public void Given_sku_created_and_stock_added_messages_When_projected()
        {
            _stockCreatedEvent = new SkuStockCreated(Guid.NewGuid(), Guid.NewGuid(), 100, TimeSpan.FromDays(2));
            _stockAddedEvent = new StockAdded(_stockCreatedEvent.SourceId, 15, "test pack");

            ProjectionBuilder.Handle(_stockCreatedEvent);
            ProjectionBuilder.Handle(_stockAddedEvent);
        }

        [Test]
        public void Then_history_fields_are_filled()
        {
            using (var context = ContextFactory())
            {
                var row = context.StockHistory.Find(_stockAddedEvent.SourceId, (long) 2);

                Assert.AreEqual(2, row.Number);
                Assert.AreEqual(_stockAddedEvent.SourceId, row.StockId);
                Assert.AreEqual(StockOperation.Added, row.Operation);
                Assert.AreEqual(_stockAddedEvent.Quantity, row.Quanity);

                Assert.AreEqual(0, row.OldReservedQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity, row.OldTotalQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity, row.OldAvailableQuantity);

                Assert.AreEqual(0, row.NewReservedQuantity);
                Assert.AreEqual(_stockAddedEvent.Quantity + _stockCreatedEvent.Quantity, row.NewTotalQuantity);
                Assert.AreEqual(_stockAddedEvent.Quantity + _stockCreatedEvent.Quantity, row.NewAvailableQuantity);
            }
        }

        [Test]
        public void Then_history_new_row_is_added()
        {
            using (var context = ContextFactory()) Assert.NotNull(context.StockHistory.Find(_stockAddedEvent.SourceId, (long) 2));
        }

        [Test]
        public void Then_stock_entry_is_renewed()
        {
            using (var context = ContextFactory())
            {
                var row = context.SkuStocks.Find(_stockAddedEvent.SourceId);
                Assert.AreEqual(_stockAddedEvent.Quantity + _stockCreatedEvent.Quantity, row.AvailableQuantity);
                Assert.AreEqual(_stockAddedEvent.CreatedTime, row.LastModified);
                Assert.AreEqual(_stockAddedEvent.Quantity + _stockCreatedEvent.Quantity, row.TotalQuantity);
            }
        }
    }
}