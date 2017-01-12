using System;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Shop.Domain.Aggregates.SkuAggregate.Events;

namespace Shop.Tests.Unit.SkuAggregate.ProjectionBuilder
{
    [TestFixture]
    public class Sku_created_projection_tests : SkuProjectionBuilderTests
    {
        private SkuCreated _message;

        [OneTimeSetUp]
        public void Given_sku_created_message_projected()
        {
            _message = new Fixture().Create<SkuCreated>();
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
                Assert.NotNull(context.Skus.Find(_message.SourceId));
        }

        [Test]
        public void When_project_all_fields_are_filled()
        {
            using (var context = ContextFactory())
            {
                var row = context.Skus.Find(_message.SourceId);
                Assert.AreEqual(_message.SourceId, row.Id);
                Assert.AreEqual(_message.Article, row.Article);
                Assert.AreEqual(_message.Name, row.Name);
                Assert.AreEqual(_message.Number, row.Number);
                Assert.AreEqual(_message.CreatedTime, row.Created);
                Assert.AreEqual(_message.CreatedTime, row.LastModified);
                Assert.AreEqual(_message.Price, row.Price);
            }
        }



    }
}