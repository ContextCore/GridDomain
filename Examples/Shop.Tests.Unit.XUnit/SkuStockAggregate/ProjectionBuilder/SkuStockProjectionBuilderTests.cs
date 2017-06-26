using System.Reflection;
using Shop.Infrastructure;
using Shop.ReadModel;
using Shop.ReadModel.Context;
using Xunit;

namespace Shop.Tests.Unit.XUnit.SkuStockAggregate.ProjectionBuilder
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