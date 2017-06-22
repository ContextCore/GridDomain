using System;
using System.Threading.Tasks;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.ProjectionBuilder
{
   
    public class SkuStock_reserve_taken_errors_test : SkuStockProjectionBuilderTests
    {
       [Fact]
        public async Task Given_no_reserve_When_reserve_taken_projected_Then_error_occurs()
        {
            var stockId = Guid.NewGuid();
            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            var reserveTaken = new StockReserveTaken(stockId, Guid.NewGuid());

            await ProjectionBuilder.Handle(stockCreatedEvent);
            await Assert.ThrowsAsync<ReserveEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveTaken));
        }

       [Fact]
        public async Task Given_no_stock_When_reserve_taken_projected_Then_error_occurs()
        {
            var reserveTaken = new StockReserveTaken(Guid.NewGuid(), Guid.NewGuid());
            await Assert.ThrowsAsync<SkuStockEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveTaken));
        }

       [Fact]
        public async Task Given_sku_created_and_stock_added_and_stock_reserved_messages_When_projected()
        {
            var stockId = Guid.NewGuid();

            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            var stockReservedEvent = new StockReserved(stockId, Guid.NewGuid(), DateTime.Now.AddDays(1), 7);
            var stockReserveTakenEvent = new StockReserveTaken(stockId, stockReservedEvent.ReserveId);

            await ProjectionBuilder.Handle(stockCreatedEvent);
            await ProjectionBuilder.Handle(stockReservedEvent);
            await ProjectionBuilder.Handle(stockReserveTakenEvent);
            await Assert.ThrowsAsync<ReserveEntryNotFoundException>(() => ProjectionBuilder.Handle(stockReserveTakenEvent));
        }
    }
}