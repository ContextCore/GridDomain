using System;
using NUnit.Framework;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    [TestFixture]
    public class SkuStock_reserve_taken_errors_test : SkuStockProjectionBuilderTests
    {

        [Test]
        public void Given_sku_created_and_stock_added_and_stock_reserved_messages_When_projected()
        {
            var stockId = Guid.NewGuid();

            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            var stockReservedEvent = new StockReserved(stockId, Guid.NewGuid(), DateTime.Now.AddDays(1), 7);
            var stockReserveTakenEvent = new StockReserveTaken(stockId, stockReservedEvent.ClientId);

            ProjectionBuilder.Handle(stockCreatedEvent);
            ProjectionBuilder.Handle(stockReservedEvent);
            ProjectionBuilder.Handle(stockReserveTakenEvent);

            Assert.Throws<ReserveEntryNotFoundException>(() => ProjectionBuilder.Handle(stockReserveTakenEvent));
        }


        [Test]
        public void Given_no_reserve_When_reserve_taken_projected_Then_error_occurs()
        {
            var stockId = Guid.NewGuid();
            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            var reserveTaken = new StockReserveTaken(stockId, Guid.NewGuid());

            ProjectionBuilder.Handle(stockCreatedEvent);
            Assert.Throws<ReserveEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveTaken));
        }

        [Test]
        public void Given_no_stock_When_reserve_taken_projected_Then_error_occurs()
        {
            var reserveTaken = new StockReserveTaken(Guid.NewGuid(), Guid.NewGuid());
            Assert.Throws<SkuStockEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveTaken));
        }

    }
}