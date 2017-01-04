using System;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    [TestFixture]
    public class SkuStock_double_reserved_tests : SkuStockProjectionBuilderTests
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
        public void When_project_reserve_again_error_is_occured()
        {
            Assert.Throws<ArgumentException>(() => ProjectionBuilder.Handle(_stockReservedEvent));
        }
    }
}