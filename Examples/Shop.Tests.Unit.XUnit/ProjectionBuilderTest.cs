using System;
using GridDomain.Common;
using Microsoft.EntityFrameworkCore;

namespace Shop.Tests.Unit.XUnit
{
    public class ProjectionBuilderTest<TContext, TBuilder> where TContext : DbContext
    {
        private static readonly Random Random = new Random();
        private readonly string DatabaseName;
        protected DbContextOptions<TContext> Options;

        protected ProjectionBuilderTest(string dbName = null)
        {
            DatabaseName = dbName ?? GetType().BeautyName() + Random.Next();
            Options = new DbContextOptionsBuilder<TContext>().UseInMemoryDatabase(DatabaseName).Options;
//            Console.WriteLine($"Generated options for test in-memory db '{DatabaseName}'");
        }

        protected TBuilder ProjectionBuilder { get; set; }
        protected Func<TContext> ContextFactory { get; set; }
    }
}