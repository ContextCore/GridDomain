using System;
using Microsoft.EntityFrameworkCore;
using Shop.ReadModel;
using Shop.ReadModel.Context;

namespace Shop.Tests.Unit.XUnit.SkuAggregate.ProjectionBuilder
{
    public class SkuProjectionBuilderTests : ProjectionBuilderTest<ShopDbContext, SkuProjectionBuilder>
    {
        protected SkuProjectionBuilderTests(string dbName = null)
        {
            ContextFactory = options => new ShopDbContext(options);
            ProjectionBuilder = new SkuProjectionBuilder(CreateContext);
        }

    }
}