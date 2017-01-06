using System;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    [TestFixture]
    public class SkuStock_reserve_expired_errors_tests : SkuStockProjectionBuilderTests
    {

        [Test]
        public void Given_sku_created_and_stock_added_and_stock_reserved_messages_When_stock_expires_projected_twice()
        {
            var stockId = Guid.NewGuid();

            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 10, TimeSpan.FromDays(2));
            var stockReservedEvent = new StockReserved(stockId, Guid.NewGuid(), DateTime.Now.AddDays(1), 7);
            var reserveExpiredEvent = new ReserveExpired(stockId, stockReservedEvent.ReserveId);

            ProjectionBuilder.Handle(stockCreatedEvent);
            ProjectionBuilder.Handle(stockReservedEvent);
            ProjectionBuilder.Handle(reserveExpiredEvent);
            Assert.Throws<ReserveEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveExpiredEvent));
        }

        [Test]
        public void Given_no_reserve_When_reserve_expires_projected_Then_error_occurs()
        {
            var stockId = Guid.NewGuid();

            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            var reserveExpired = new ReserveExpired(stockId, Guid.NewGuid());

            ProjectionBuilder.Handle(stockCreatedEvent);
            Assert.Throws<ReserveEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveExpired));
        }

        [Test]
        public void Given_no_stock_When_reserve_cancel_projected_Then_error_occurs()
        {
            var stockId = Guid.NewGuid();

            var reserveExpired = new ReserveExpired(stockId, Guid.NewGuid());
            Assert.Throws<SkuStockEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveExpired));
        }

    }
}