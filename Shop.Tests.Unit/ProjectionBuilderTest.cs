using System;
using GridDomain.Common;
using Microsoft.EntityFrameworkCore;

namespace Shop.Tests.Unit
{
    public class ProjectionBuilderTest<TContext, TBuilder> where TContext:DbContext
    {
        protected readonly DbContextOptions<TContext> Options;
        protected TBuilder ProjectionBuilder { get; set; }
        protected  Func<TContext> ContextFactory { get; set; }
        protected ProjectionBuilderTest(string dbName = null)
        {
            string name = dbName ?? this.GetType().BeautyName();
            Options = new DbContextOptionsBuilder<TContext>()
                                                .UseInMemoryDatabase(name)
                                                .Options;
        }
    }
}