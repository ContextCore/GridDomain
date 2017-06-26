using System;
using System.Threading.Tasks;
using Shop.Domain.Aggregates.OrderAggregate.Events;
using Shop.ReadModel.Context;
using Xunit;

namespace Shop.Tests.Unit.OrderAggregate.ProjectionBuilders
{
   
    public class Order_created_projection_test : Order_projection_builder_test
    {
       
        public Order_created_projection_test()// Given_order_created_message()
        {
            _msg = new OrderCreated(Guid.NewGuid(), 123, Guid.NewGuid());
            var user = new User {Id = _msg.User, Login = "test"};
            using (var context = CreateContext())
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            ProjectionBuilder.Handle(_msg).Wait();
        }

        private OrderCreated _msg;

       [Fact]
        public void Should_created_order_line()
        {
            using (var context = CreateContext())
            {
                var row = context.Orders.Find(_msg.SourceId);
                Assert.NotNull(row);
            }
        }

       [Fact]
        public async Task Should_throw_exceptions_on_double_event()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => ProjectionBuilder.Handle(_msg));
        }

       [Fact]
        public void Should_write_all_fields()
        {
            using (var context = CreateContext())
            {
                var row = context.Orders.Find(_msg.SourceId);

                Assert.Equal(_msg.SourceId, row.Id);
                Assert.Equal(_msg.Number, row.Number);
                Assert.Equal(_msg.Status, row.Status);
                Assert.Equal(_msg.User, row.UserId);
                Assert.Equal(_msg.CreatedTime, row.Created);
            }
        }
    }
}