using Shop.ReadModel;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.XUnit.OrderAggregate.ProjectionBuilders
{
    public class Order_projection_builder_test : ProjectionBuilderTest<ShopDbContext, OrdersProjectionBuilder>
    {
        public Order_projection_builder_test()
        {
            ContextFactory = options => new ShopDbContext(options);
            ProjectionBuilder = new OrdersProjectionBuilder(CreateContext);
        }
    }
}