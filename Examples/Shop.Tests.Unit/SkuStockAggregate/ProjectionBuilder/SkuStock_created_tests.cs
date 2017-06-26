using System;
using System.Threading.Tasks;
using GridDomain.Tests.Common;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;
using Shop.ReadModel.Context;
using Xunit;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    
    public class SkuStock_created_tests : SkuStockProjectionBuilderTests
    {
        private SkuStockCreated _message;

        public SkuStock_created_tests() // Given_sku_created_message_When_projected()
        {
            _message = new Fixture().Create<SkuStockCreated>();
            ProjectionBuilder.Handle(_message).Wait();
        }

        [Fact]
        public void Then_new_sku_stock_row_is_added()
        {
            using (var context = CreateContext())
            {
                Assert.NotNull(context.SkuStocks.Find(_message.SourceId));
            }
        }

        [Fact]
        public void Then_new_stock_history_row_is_added()
        {
            using (var context = CreateContext())
            {
                Assert.NotNull(context.StockHistory.Find(_message.SourceId, (long) 1));
            }
        }

        [Fact]
        public void Then_new_stock_history_row_is_filled()
        {
            using (var context = CreateContext())
            {
                var row = context.StockHistory.Find(_message.SourceId, (long) 1);
                Assert.Equal(1, row.Number);
                Assert.Equal(StockOperation.Created, row.Operation);
                Assert.Equal(_message.Quantity, row.Quanity);
                Assert.Equal(_message.SourceId, row.StockId);

                Assert.Equal(0, row.OldAvailableQuantity);
                Assert.Equal(0, row.OldReservedQuantity);
                Assert.Equal(0, row.OldTotalQuantity);

                Assert.Equal(_message.Quantity, row.NewAvailableQuantity);
                Assert.Equal(0, row.NewReservedQuantity);
                Assert.Equal(_message.Quantity, row.NewTotalQuantity);
            }
        }

        [Fact]
        public void Then_sku_row_fields_are_filled()
        {
            using (var context = CreateContext())
            {
                var row = context.SkuStocks.Find(_message.SourceId);
                Assert.Equal(_message.SourceId, row.Id);
                Assert.Equal(_message.Quantity, row.AvailableQuantity);
                Assert.Equal(_message.CreatedTime, row.Created);
                Assert.Equal(0, row.CustomersReservationsTotal);
                Assert.Equal(_message.CreatedTime, row.LastModified);
                Assert.Equal(0, row.ReservedQuantity);
                Assert.Equal(_message.SkuId, row.SkuId);
                Assert.Equal(_message.Quantity, row.TotalQuantity);
            }
        }

        [Fact]
        public async Task When_project_again_error_occures()
        {
            await ProjectionBuilder.Handle(_message)
                                   .ShouldThrow<ArgumentException>();
        }
    }
}