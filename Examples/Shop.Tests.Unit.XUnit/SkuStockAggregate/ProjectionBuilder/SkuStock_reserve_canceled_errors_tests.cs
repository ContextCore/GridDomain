using System;
using System.Threading.Tasks;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.ProjectionBuilder
{
   
    public class SkuStock_reserve_canceled_errors_tests : SkuStockProjectionBuilderTests
    {
       [Fact]
        public async Task  Given_no_reserve_When_reserve_cancel_projected_Then_error_occurs()
        {
            var stockId = Guid.NewGuid();

            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            var reserveCanceledEvent = new ReserveCanceled(stockId, Guid.NewGuid());

            await ProjectionBuilder.Handle(stockCreatedEvent);
            await Assert.ThrowsAsync<ReserveEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveCanceledEvent));
        }

       [Fact]
        public async Task Given_no_stock_When_reserve_cancel_projected_Then_error_occurs()
        {
            var stockId = Guid.NewGuid();

            var reserveCanceledEvent = new ReserveCanceled(stockId, Guid.NewGuid());

            await Assert.ThrowsAsync<SkuStockEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveCanceledEvent));
        }

       [Fact]
        public async Task Given_sku_created_and_stock_added_and_stock_reserved_messages_When_projected_twice()
        {
            var stockId = Guid.NewGuid();

            var stockCreatedEvent = new SkuStockCreated(stockId, Guid.NewGuid(), 1, TimeSpan.FromDays(2));
            var stockAddedEvent = new StockAdded(stockId, 15, "test pack");
            var stockReservedEvent = new StockReserved(stockId, Guid.NewGuid(), DateTime.Now.AddDays(1), 7);
            var reserveCanceledEvent = new ReserveCanceled(stockId, stockReservedEvent.ReserveId);

            await ProjectionBuilder.Handle(stockCreatedEvent);
            await ProjectionBuilder.Handle(stockAddedEvent);
            await ProjectionBuilder.Handle(stockReservedEvent);
            await ProjectionBuilder.Handle(reserveCanceledEvent);
            await Assert.ThrowsAsync<ReserveEntryNotFoundException>(() => ProjectionBuilder.Handle(reserveCanceledEvent));
        }
    }
}