using Shop.Infrastructure;
using Shop.ReadModel;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.SkuStockAggregate.ProjectionBuilder
{

    public class SkuStockProjectionBuilderTests : ProjectionBuilderTest<ShopDbContext, SkuStockProjectionBuilder>
    {
        
        protected SkuStockProjectionBuilderTests(string dbName = null)
        {
            ContextFactory = options => new ShopDbContext(options);
            ProjectionBuilder = new SkuStockProjectionBuilder(CreateContext, new InMemorySequenceProvider());
        }
    }
}