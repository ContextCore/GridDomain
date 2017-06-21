using System;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    [TestFixture]
    public class SkuStock_reserved_wihout_sku_entry_tests : SkuStockProjectionBuilderTests
    {
        private StockReserved _stockReservedEvent;

        [OneTimeSetUp]
        public void Given_stock_reserved_message_without_stock_created_When_projected()
        {
            _stockReservedEvent = new StockReserved(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now.AddDays(1), 7);
        }

        [Test]
        public void When_projected_then_error_occures()
        {
            Assert.Throws<SkuStockEntryNotFoundException>(() => ProjectionBuilder.Handle(_stockReservedEvent));
        }
    }
}