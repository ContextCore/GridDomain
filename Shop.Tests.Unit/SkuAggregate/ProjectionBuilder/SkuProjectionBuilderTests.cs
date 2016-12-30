using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shop.ReadModel;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.SkuAggregate.ProjectionBuilder
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
