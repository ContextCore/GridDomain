using System;
using GridDomain.Common;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Shop.Tests.Unit
{
    public class ProjectionBuilderTest<TContext, TBuilder> where TContext : DbContext
    {
        private static readonly Random Random = new Random();
        private readonly string DatabaseName;
        protected DbContextOptions<TContext> Options;

        protected ProjectionBuilderTest(string dbName = null)
        {
            DatabaseName = dbName ?? GetType()
                .BeautyName() + Random.Next();
        }

        protected TBuilder ProjectionBuilder { get; set; }
        protected Func<TContext> ContextFactory { get; set; }

        [OneTimeSetUp]
        public void Generate_db_options()
        {
            Options = new DbContextOptionsBuilder<TContext>().UseInMemoryDatabase(DatabaseName)
                                                             .Options;

            Console.WriteLine($"Generated options for test in-memory db '{DatabaseName}'");
        }
    }
}