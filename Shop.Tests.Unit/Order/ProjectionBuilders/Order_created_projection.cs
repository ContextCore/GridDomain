using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.ReadModel;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.Order.ProjectionBuilders
{

    [TestFixture]
    public class Order_created_projection_test : Order_projection_builder_test
    {
        private OrderCreated _msg;

        [SetUp]
        public void Given_order_created_message()
        {
            _msg = new OrderCreated(Guid.NewGuid(), 123, Guid.NewGuid());
            var user = new User() {Id = _msg.User, Login = "test"};
            using (var context = ContextFactory())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            ProjectionBuilder.Handle(_msg);
        }

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

                Assert.AreEqual(_msg.SourceId,row.Id);
                Assert.AreEqual(_msg.Number,row.Number);
                Assert.AreEqual(_msg.Status,row.Status);
                Assert.AreEqual(_msg.User,row.UserId);
                Assert.AreEqual(_msg.CreatedTime,row.Created);
            }
        }
    }


    public class Order_projection_builder_test
    {
        protected readonly OrdersProjectionBuilder ProjectionBuilder;
        protected readonly Func<ShopDbContext> ContextFactory;

        public Order_projection_builder_test()
        {
            var options = new DbContextOptionsBuilder<ShopDbContext>()
                             .UseInMemoryDatabase("Order_created_projection")
                             .Options;

            ContextFactory = () => new ShopDbContext(options);
            ProjectionBuilder = new OrdersProjectionBuilder(ContextFactory);
        }
    }
}
