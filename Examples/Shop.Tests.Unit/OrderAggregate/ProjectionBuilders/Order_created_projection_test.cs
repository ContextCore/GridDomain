using System;
using NUnit.Framework;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.OrderAggregate.ProjectionBuilders
{
    [TestFixture]
    public class Order_created_projection_test : Order_projection_builder_test
    {
        [SetUp]
        public void Given_order_created_message()
        {
            _msg = new OrderCreated(Guid.NewGuid(), 123, Guid.NewGuid());
            var user = new User {Id = _msg.User, Login = "test"};
            using (var context = ContextFactory())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            ProjectionBuilder.Handle(_msg);
        }

        private OrderCreated _msg;

        [Test]
        public void Should_created_order_line()
        {
            using (var context = ContextFactory())
            {
                var row = context.Orders.Find(_msg.SourceId);
                Assert.NotNull(row);
            }
        }

        [Test]
        public void Should_throw_exceptions_on_double_event()
        {
            Assert.Throws<ArgumentException>(() => ProjectionBuilder.Handle(_msg));
        }

        [Test]
        public void Should_write_all_fields()
        {
            using (var context = ContextFactory())
            {
                var row = context.Orders.Find(_msg.SourceId);

                Assert.AreEqual(_msg.SourceId, row.Id);
                Assert.AreEqual(_msg.Number, row.Number);
                Assert.AreEqual(_msg.Status, row.Status);
                Assert.AreEqual(_msg.User, row.UserId);
                Assert.AreEqual(_msg.CreatedTime, row.Created);
            }
        }
    }
}