using System.Linq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    [TestFixture]
    public class SkuStock_added_two_times_tests : SkuStockProjectionBuilderTests
    {
        private StockAdded _stockAddedEvent;
        private SkuStockCreated _stockCreatedEvent;

        [OneTimeSetUp]
        public void Given_sku_created_message_double_projected()
        {
            _stockCreatedEvent = new Fixture().Create<SkuStockCreated>();
            _stockAddedEvent = new StockAdded(_stockCreatedEvent.SourceId, 15, "test pack");

            ProjectionBuilder.Handle(_stockCreatedEvent);
            ProjectionBuilder.Handle(_stockAddedEvent);
            ProjectionBuilder.Handle(_stockAddedEvent);
        }

        [Test]
        public void When_project_again_additioanal_transaction_occures()
        {
            using (var context = ContextFactory()) Assert.AreEqual(3, context.StockHistory.Count());
        }
    }
}