using Shop.Infrastructure;
using Shop.ReadModel;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{
    public class SkuStockProjectionBuilderTests : ProjectionBuilderTest<ShopDbContext, SkuStockProjectionBuilder>
    {
        protected SkuStockProjectionBuilderTests(string dbName = null) : base(dbName)
        {
            ContextFactory = () => new ShopDbContext(Options);
            ProjectionBuilder = new SkuStockProjectionBuilder(ContextFactory, new InMemorySequenceProvider());
        }
    }
}