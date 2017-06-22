using Shop.ReadModel;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.XUnit.SkuAggregate.ProjectionBuilder
{
    public class SkuProjectionBuilderTests : ProjectionBuilderTest<ShopDbContext, SkuProjectionBuilder>
    {
        protected SkuProjectionBuilderTests(string dbName = null) : base(dbName)
        {
            ContextFactory = () => new ShopDbContext(Options);
            ProjectionBuilder = new SkuProjectionBuilder(ContextFactory);
        }
    }
}