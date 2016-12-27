using System;
using GridDomain.Common;
using Microsoft.EntityFrameworkCore;
using Shop.ReadModel;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.OrderAggregate.ProjectionBuilders
{

    public class ProjectBuilderTest<TContext, TBuilder> where TContext:DbContext
    {
        protected readonly DbContextOptions<TContext> Options;
        protected TBuilder ProjectionBuilder { get; set; }
        protected  Func<TContext> ContextFactory { get; set; }

        protected ProjectBuilderTest(string dbName = null)
        {
            string name = dbName ?? this.GetType().BeautyName();
            Options = new DbContextOptionsBuilder<TContext>()
                                        .UseInMemoryDatabase(name)
                                        .Options;
        }
    }

    public class Order_projection_builder_test :
        ProjectBuilderTest<ShopDbContext, OrdersProjectionBuilder>
    {
        public Order_projection_builder_test()
        {
            ContextFactory = () => new ShopDbContext(Options);
            ProjectionBuilder = new OrdersProjectionBuilder(ContextFactory);
        }
    }

    public class Account_projection_builder_test :
      ProjectBuilderTest<ShopDbContext, AccountProjectionBuilder>
    {
        public Account_projection_builder_test()
        {
            ContextFactory = () => new ShopDbContext(Options);
            ProjectionBuilder = new AccountProjectionBuilder(ContextFactory);
        }
    }
}