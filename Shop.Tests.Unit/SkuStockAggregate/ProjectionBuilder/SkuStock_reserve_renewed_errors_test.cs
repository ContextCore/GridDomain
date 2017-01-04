using System;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    [TestFixture]
    public class SkuStock_reserve_renewed_errors_test : SkuStockProjectionBuilderTests
    {

        [Test]
        public void Given_sku_created_and_reserved_and_renewed_messages_When_projected()
        {
            var stockId = Guid.NewGuid();

            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            var stockReservedEvent = new StockReserved(stockId, Guid.NewGuid(), DateTime.Now.AddDays(1), 7);
            var reserveRenewed = new ReserveRenewed(stockId, stockReservedEvent.ClientId);

            ProjectionBuilder.Handle(stockCreatedEvent);
            ProjectionBuilder.Handle(stockReservedEvent);
            ProjectionBuilder.Handle(reserveRenewed);

            Assert.Throws<ReserveEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveRenewed));
        }

        [Test]
        public void Given_no_reserve_When_reserve_renewed_projected_Then_error_occurs()
        {
            var stockId = Guid.NewGuid();
            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            var reserveRenewed = new ReserveRenewed(stockId, Guid.NewGuid());

            ProjectionBuilder.Handle(stockCreatedEvent);
            Assert.Throws<ReserveEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveRenewed));
        }

        [Test]
        public void Given_no_stock_When_reserve_taken_projected_Then_error_occurs()
        {
            var reserveRenewed = new ReserveRenewed(Guid.NewGuid(), Guid.NewGuid());
            Assert.Throws<SkuStockEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveRenewed));
        }

    }
}