using System;
using System.Threading.Tasks;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel;
using Xunit;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    
    public class SkuStock_reserve_renewed_errors_test : SkuStockProjectionBuilderTests
    {
       [Fact]
        public async Task Given_no_reserve_When_reserve_renewed_projected_Then_error_occurs()
        {
            var stockId = Guid.NewGuid();
            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            var reserveRenewed = new ReserveRenewed(stockId, Guid.NewGuid());

            await ProjectionBuilder.Handle(stockCreatedEvent);
            await Assert.ThrowsAsync<ReserveEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveRenewed));
        }

       [Fact]
        public async Task Given_no_stock_When_reserve_taken_projected_Then_error_occurs()
        {
            var reserveRenewed = new ReserveRenewed(Guid.NewGuid(), Guid.NewGuid());
            await Assert.ThrowsAsync<SkuStockEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveRenewed));
        }

       [Fact]
        public async Task Given_sku_created_and_reserved_and_renewed_messages_When_projected()
        {
            var stockId = Guid.NewGuid();

            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            var stockReservedEvent = new StockReserved(stockId, Guid.NewGuid(), DateTime.Now.AddDays(1), 7);
            var reserveRenewed = new ReserveRenewed(stockId, stockReservedEvent.ReserveId);

            await ProjectionBuilder.Handle(stockCreatedEvent);
            await ProjectionBuilder.Handle(stockReservedEvent);
            await ProjectionBuilder.Handle(reserveRenewed);
            await Assert.ThrowsAsync<ReserveEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveRenewed));
        }
    }
}