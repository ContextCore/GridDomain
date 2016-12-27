using System;
using Microsoft.EntityFrameworkCore;
using Shop.ReadModel;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.OrderAggregate.ProjectionBuilders
{
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