using System;
using GridDomain.Common;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Shop.Tests.Unit
{
    public class ProjectionBuilderTest<TContext, TBuilder> where TContext:DbContext
    {
        protected DbContextOptions<TContext> Options;
        protected TBuilder ProjectionBuilder { get; set; }
        protected  Func<TContext> ContextFactory { get; set; }
        private static readonly Random Random = new Random();
        private string DatabaseName;

        protected ProjectionBuilderTest(string dbName = null)
        {
            DatabaseName = dbName ?? this.GetType().BeautyName() + Random.Next();
            
        }

        [OneTimeSetUp]
        public void Generate_db_options()
        {
            Options = new DbContextOptionsBuilder<TContext>()
                                             .UseInMemoryDatabase(DatabaseName)
                                             .Options;

            Console.WriteLine($"Generated options for test in-memory db '{DatabaseName}'");
        } 

    }
}