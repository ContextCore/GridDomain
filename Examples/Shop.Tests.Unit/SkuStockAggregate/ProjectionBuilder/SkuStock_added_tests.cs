using System;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel.Context;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.ProjectionBuilder
{

    
    public class SkuStock_added_tests : SkuStockProjectionBuilderTests
    {
        private StockAdded _stockAddedEvent;
        private SkuStockCreated _stockCreatedEvent;

        public SkuStock_added_tests()// Given_sku_created_and_stock_added_messages_When_projected()
        {
            _stockCreatedEvent = new SkuStockCreated(Guid.NewGuid(), Guid.NewGuid(), 100, TimeSpan.FromDays(2));
            _stockAddedEvent = new StockAdded(_stockCreatedEvent.SourceId, 15, "test pack");

            ProjectionBuilder.Handle(_stockCreatedEvent).Wait();
            ProjectionBuilder.Handle(_stockAddedEvent).Wait();
        }

       [Fact]
        public void Then_history_fields_are_filled()
        {
            using (var context = CreateContext())
            {
                var row = context.StockHistory.Find(_stockAddedEvent.SourceId, (long) 2);

                Assert.Equal(2, row.Number);
                Assert.Equal(_stockAddedEvent.SourceId, row.StockId);
                Assert.Equal(StockOperation.Added, row.Operation);
                Assert.Equal(_stockAddedEvent.Quantity, row.Quanity);

                Assert.Equal(0, row.OldReservedQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity, row.OldTotalQuantity);
                Assert.Equal(_stockCreatedEvent.Quantity, row.OldAvailableQuantity);

                Assert.Equal(0, row.NewReservedQuantity);
                Assert.Equal(_stockAddedEvent.Quantity + _stockCreatedEvent.Quantity, row.NewTotalQuantity);
                Assert.Equal(_stockAddedEvent.Quantity + _stockCreatedEvent.Quantity, row.NewAvailableQuantity);
            }
        }

       [Fact]
        public void Then_history_new_row_is_added()
        {
            using (var context = CreateContext())
            {
                Assert.NotNull(context.StockHistory.Find(_stockAddedEvent.SourceId, (long) 2));
            }
        }

       [Fact]
        public void Then_stock_entry_is_renewed()
        {
            using (var context = CreateContext())
            {
                var row = context.SkuStocks.Find(_stockAddedEvent.SourceId);
                Assert.Equal(_stockAddedEvent.Quantity + _stockCreatedEvent.Quantity, row.AvailableQuantity);
                Assert.Equal(_stockAddedEvent.CreatedTime, row.LastModified);
                Assert.Equal(_stockAddedEvent.Quantity + _stockCreatedEvent.Quantity, row.TotalQuantity);
            }
        }
    }
}