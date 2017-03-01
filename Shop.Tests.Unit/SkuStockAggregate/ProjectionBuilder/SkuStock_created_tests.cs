using System;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    [TestFixture]
    public class SkuStock_created_tests : SkuStockProjectionBuilderTests
    {
        private SkuStockCreated _message;

        [OneTimeSetUp]
        public void Given_sku_created_message_When_projected()
        {
            _message = new Fixture().Create<SkuStockCreated>();
            ProjectionBuilder.Handle(_message);
        }

        [Test]
        public void Then_new_sku_stock_row_is_added()
        {
            using (var context = ContextFactory())
            {
                Assert.NotNull(context.SkuStocks.Find(_message.SourceId));
            }
        }

        [Test]
        public void Then_new_stock_history_row_is_added()
        {
            using (var context = ContextFactory())
            {
                Assert.NotNull(context.StockHistory.Find(_message.SourceId, (long) 1));
            }
        }

        [Test]
        public void Then_new_stock_history_row_is_filled()
        {
            using (var context = ContextFactory())
            {
                var row = context.StockHistory.Find(_message.SourceId, (long) 1);
                Assert.AreEqual(1, row.Number);
                Assert.AreEqual(StockOperation.Created, row.Operation);
                Assert.AreEqual(_message.Quantity, row.Quanity);
                Assert.AreEqual(_message.SourceId, row.StockId);

                Assert.AreEqual(0, row.OldAvailableQuantity);
                Assert.AreEqual(0, row.OldReservedQuantity);
                Assert.AreEqual(0, row.OldTotalQuantity);

                Assert.AreEqual(_message.Quantity, row.NewAvailableQuantity);
                Assert.AreEqual(0, row.NewReservedQuantity);
                Assert.AreEqual(_message.Quantity, row.NewTotalQuantity);
            }
        }

        [Test]
        public void Then_sku_row_fields_are_filled()
        {
            using (var context = ContextFactory())
            {
                var row = context.SkuStocks.Find(_message.SourceId);
                Assert.AreEqual(_message.SourceId, row.Id);
                Assert.AreEqual(_message.Quantity, row.AvailableQuantity);
                Assert.AreEqual(_message.CreatedTime, row.Created);
                Assert.AreEqual(0, row.CustomersReservationsTotal);
                Assert.AreEqual(_message.CreatedTime, row.LastModified);
                Assert.AreEqual(0, row.ReservedQuantity);
                Assert.AreEqual(_message.SkuId, row.SkuId);
                Assert.AreEqual(_message.Quantity, row.TotalQuantity);
            }
        }

        [Test]
        public void When_project_again_error_occures()
        {
            Assert.Throws<ArgumentException>(() => ProjectionBuilder.Handle(_message));
        }
    }
}