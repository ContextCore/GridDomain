using System;
using System.Threading.Tasks;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel;
using Xunit;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    
    public class SkuStock_reserve_expired_errors_tests : SkuStockProjectionBuilderTests
    {
       [Fact]
        public async Task Given_no_reserve_When_reserve_expires_projected_Then_error_occurs()
        {
            var stockId = Guid.NewGuid();

            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            var reserveExpired = new ReserveExpired(stockId, Guid.NewGuid());

            await ProjectionBuilder.Handle(stockCreatedEvent);
            await Assert.ThrowsAsync<ReserveEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveExpired));
        }

       [Fact]
        public async Task Given_no_stock_When_reserve_cancel_projected_Then_error_occurs()
        {
            var stockId = Guid.NewGuid();

            var reserveExpired = new ReserveExpired(stockId, Guid.NewGuid());
            await Assert.ThrowsAsync<SkuStockEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveExpired));
        }

       [Fact]
        public async Task Given_sku_created_and_stock_added_and_stock_reserved_messages_When_stock_expires_projected_twice()
        {
            var stockId = Guid.NewGuid();

            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 10, TimeSpan.FromDays(2));
            var stockReservedEvent = new StockReserved(stockId, Guid.NewGuid(), DateTime.Now.AddDays(1), 7);
            var reserveExpiredEvent = new ReserveExpired(stockId, stockReservedEvent.ReserveId);

            await ProjectionBuilder.Handle(stockCreatedEvent);
            await ProjectionBuilder.Handle(stockReservedEvent);
            await ProjectionBuilder.Handle(reserveExpiredEvent);
            await Assert.ThrowsAsync<ReserveEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveExpiredEvent));
        }
    }
}