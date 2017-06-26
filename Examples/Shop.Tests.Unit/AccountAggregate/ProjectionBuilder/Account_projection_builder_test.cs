using Shop.ReadModel;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.XUnit.AccountAggregate.ProjectionBuilder
{
    public class Account_projection_builder_test : ProjectionBuilderTest<ShopDbContext, AccountProjectionBuilder>
    {
        public Account_projection_builder_test()
        {
            ContextFactory = options => new ShopDbContext(options);
            ProjectionBuilder = new AccountProjectionBuilder(CreateContext);
        }
    }
}