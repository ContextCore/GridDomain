using System;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    [TestFixture]
    public class SkuStock_added_without_sku_created_tests : SkuStockProjectionBuilderTests
    {
        private StockAdded _stockAddedEvent;

        [OneTimeSetUp]
        public void Given_sku_created_message_double_projected()
        {
            _stockAddedEvent = new StockAdded(Guid.NewGuid(), 15, "test pack");
        }

        [Test]
        public void When_project_Then_error_occures()
        {
            Assert.Throws<SkuStockEntryNotFoundException>(() => ProjectionBuilder.Handle(_stockAddedEvent));
        }
    }
}