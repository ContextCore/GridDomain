using System;
using GridDomain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Shop.Tests.Unit.XUnit
{
    [Collection("ProjectionTests")]
    public class ProjectionBuilderTest<TContext, TBuilder> where TContext : DbContext
    {
        private DbContextOptions<TContext> Options { get; }

        protected ProjectionBuilderTest()
        {
            Options = CreateNewContextOptions();
        }

        private DbContextOptions<TContext> CreateNewContextOptions()
        {
            // Create a fresh service provider, and therefore a fresh 
            // InMemory database instance.
            var serviceProvider = new ServiceCollection()
                                        .AddEntityFrameworkInMemoryDatabase()
                                        .BuildServiceProvider();

            // Create a new options instance telling the context to use an
            // InMemory database and the new service provider.
            var builder = new DbContextOptionsBuilder<TContext>();
            builder.UseInMemoryDatabase(GetType().BeautyName() + new Random().Next())
                   .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }

        protected Func<DbContextOptions<TContext>, TContext> ContextFactory { get; set; }
        protected TBuilder ProjectionBuilder { get; set; }

        protected TContext CreateContext()
        {
            return ContextFactory(Options);
        }
    }
}