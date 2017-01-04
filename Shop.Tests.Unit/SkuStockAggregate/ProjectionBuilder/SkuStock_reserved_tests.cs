using System;
using System.Linq;
using GridDomain.Logging;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    [TestFixture]
    public class SkuStock_reserved_tests : SkuStockProjectionBuilderTests
    {
        private StockAdded _stockAddedEvent;
        private SkuStockCreated _stockCreatedEvent;
        private StockReserved _stockReservedEvent;

        [OneTimeSetUp]
        public void Given_sku_created_and_stock_added_and_stock_reserved_messages_When_projected()
        {
            _stockCreatedEvent = new SkuStockCreated(Guid.NewGuid(), Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            _stockAddedEvent = new StockAdded(_stockCreatedEvent.SourceId, 15, "test pack");
            _stockReservedEvent = new StockReserved(_stockCreatedEvent.SourceId, Guid.NewGuid(), DateTime.Now.AddDays(1),
                7);

            ProjectionBuilder.Handle(_stockCreatedEvent);
            ProjectionBuilder.Handle(_stockAddedEvent);
            ProjectionBuilder.Handle(_stockReservedEvent);
        }

        [Test]
        public void Then_history_row_is_added()
        {
            using (var context = ContextFactory())
            {
                var history = context.StockHistory.Find(_stockCreatedEvent.SourceId, 2);
                if (history != null) return;

                foreach(var hist in context.StockHistory)
                    Console.WriteLine(hist.ToPropsString());
                Assert.Fail("Cannot find history");
            }
        }

        [Test]
        public void Then_history_containes_rows_for_added_and_reserved_events()
        {
            using (var context = ContextFactory())
                Assert.AreEqual(2, context.StockHistory.Count());
        }

        [Test]
        public void Then_history_row_is_filled()
        {
            using (var context = ContextFactory())
            {
                //#1 is stock added history
                var history = context.StockHistory.Find(_stockCreatedEvent.SourceId, 2);
                Assert.AreEqual(_stockCreatedEvent.Quantity +
                                _stockAddedEvent.Quantity -
                                _stockReservedEvent.Quantity, history.NewAvailableQuantity);
                Assert.AreEqual(_stockReservedEvent.Quantity, history.NewReservedQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity +
                                _stockAddedEvent.Quantity, history.NewTotalQuantity);
                Assert.AreEqual(0, history.OldReservedQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity +
                                _stockAddedEvent.Quantity, history.OldTotalQuantity);
                Assert.AreEqual(_stockCreatedEvent.Quantity +
                                _stockAddedEvent.Quantity, history.OldAvailableQuantity);
                Assert.AreEqual(StockOperation.Reserved, history.Operation);
                Assert.AreEqual(_stockReservedEvent.Quantity, history.Quanity);
                Assert.AreEqual(_stockReservedEvent.SourceId, history.StockId);
                Assert.AreEqual(2, history.Number);
            }
        }

        [Test]
        public void Then_reserve_row_is_added()
        {
            using (var context = ContextFactory())
                Assert.NotNull(context.StockReserves.Find(_stockCreatedEvent.SourceId, _stockReservedEvent.ClientId));

        }

        [Test]
        public void Then_reserve_row_is_filled()
        {
            using (var context = ContextFactory())
            {
                var reserve = context.StockReserves.Find(_stockCreatedEvent.SourceId, _stockReservedEvent.ClientId);

                Assert.AreEqual(_stockReservedEvent.ClientId, reserve.CustomerId);
                Assert.AreEqual(_stockCreatedEvent.SkuId, reserve.SkuId);
                Assert.AreEqual(_stockReservedEvent.SourceId, reserve.StockId);
                Assert.AreEqual(_stockReservedEvent.CreatedTime, reserve.Created);
                Assert.AreEqual(_stockReservedEvent.ExpirationDate, reserve.ExpirationDate);
                Assert.AreEqual(_stockReservedEvent.Quantity, reserve.Quantity);
            }

        }

        [Test]
        public void Then_sku_stock_row_is_modified()
        {
            using (var context = ContextFactory())
            {
                var stock = context.SkuStocks.Find(_stockCreatedEvent.SourceId);
                Assert.AreEqual(_stockAddedEvent.Quantity +
                                _stockCreatedEvent.Quantity -
                                _stockReservedEvent.Quantity, stock.AvailableQuantity);
                Assert.AreEqual(1, stock.CustomersReservationsTotal);
                Assert.AreEqual(_stockReservedEvent.CreatedTime, stock.LastModified);
                Assert.AreEqual(_stockReservedEvent.Quantity, stock.ReservedQuantity);
                Assert.AreEqual(_stockAddedEvent.Quantity +
                                _stockCreatedEvent.Quantity, stock.TotalQuantity);

            }
        }
    }
}