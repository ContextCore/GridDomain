using System;
using System.Linq;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    [TestFixture]
    public class SkuStock_taken_errors_test : SkuStockProjectionBuilderTests
    {
        [Test]
        public void Given_no_stock_When_stock_taken_projected_Then_error_occurs()
        {
            var reserveTaken = new StockReserveTaken(Guid.NewGuid(), Guid.NewGuid());
            Assert.Throws<SkuStockEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveTaken));
        }

        [Test]
        public void Given_sku_created_and_taken_messages_When_projected_Then_another_history_is_added()
        {
            var stockId = Guid.NewGuid();

            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 7, TimeSpan.FromDays(2));
            var stockTaken = new StockTaken(stockId, 3);

            ProjectionBuilder.Handle(stockCreatedEvent);
            ProjectionBuilder.Handle(stockTaken);
            ProjectionBuilder.Handle(stockTaken);

            Assert.AreEqual(3, ContextFactory().StockHistory.Count());
            Assert.AreEqual(1, ContextFactory().SkuStocks.Find(stockId).AvailableQuantity);
        }
    }
}