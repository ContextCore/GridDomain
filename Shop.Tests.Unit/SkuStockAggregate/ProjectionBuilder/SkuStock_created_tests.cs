using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.SkuAggregate.Events;
using Shop.Domain.Aggregates.SkuStockAggregate.Events;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{


    [TestFixture]
    public class SkuStock_created_tests : SkuStockProjectionBuilderTests
    {
        private SkuStockCreated _message;

        [OneTimeSetUp]
        public void Given_sku_created_message_projected()
        {
            _message = new Fixture().Create<SkuStockCreated>();
            ProjectionBuilder.Handle(_message);
        }

        [Test]
        public void When_project_again_error_occures()
        {
            Assert.Throws<ArgumentException>(() => ProjectionBuilder.Handle(_message));
        }

        [Test]
        public void When_project_new_row_is_added()
        {
            using (var context = ContextFactory())
                Assert.NotNull(context.SkuStocks.Find(_message.SourceId));
        }

        [Test]
        public void When_project_all_fields_are_filled()
        {
            using (var context = ContextFactory())
            {
                var row = context.SkuStocks.Find(_message.SourceId);
                Assert.AreEqual(_message.SourceId, row.Id);
                Assert.AreEqual(_message.Quantity, row.AvailableQuantity);
                Assert.AreEqual(_message.CreatedTime, row.Created);
                Assert.AreEqual(0, row.CustomersReservationsTotal);
                Assert.AreEqual(_message.CreatedTime, row.LastModified);
                Assert.AreEqual(0, row.ReservedQuantity);
                Assert.AreEqual(_message.SkuId, row.SkuId);
                Assert.AreEqual(_message.Quantity, row.TotalQuantity);
            }
        }
    }
}
