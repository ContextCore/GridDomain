using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Sdk;

namespace GridDomain.Projections.EntityFrameworkCore.Tests
{
    public class ProjectionQueryTests
    {
        
     
        class ProjectionDbContext:DbContext,IProjectionDbContext
        {
            public ProjectionDbContext(DbContextOptions option):base(option)
            {
        
            }
             
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Projection>().HasKey(p => new {p.Name, p.Projector, p.Event});
            }
            
            public DbSet<Projection> Projections { get; set; }
        }
        
        [Fact]
        public async Task Given_context_with_data_When_executing_projection_query_Then_it_should_return_data()
        {
            var options = new DbContextOptionsBuilder<ProjectionDbContext>()
                .UseInMemoryDatabase(nameof(Given_context_with_data_When_executing_projection_query_Then_it_should_return_data)).Options;

            var context = new ProjectionDbContext(options);
            var query = new FindProjectionQuery(context);
            
            var projection = new Projection
                {Event = "testEvent", Name = "testName", Projector = "projector", Offset = 10};
            context.Projections.Add(projection);
            await context.SaveChangesAsync();

            var res =  await query.Execute(projection.Name, projection.Projector, projection.Event);
            Assert.Equal(projection,res);
        }

        [Fact]
        public async Task Given_empty_context_When_executing_projection_query_Then_it_should_return_Null()
        {
            var options = new DbContextOptionsBuilder<ProjectionDbContext>()
                .UseInMemoryDatabase(nameof(Given_empty_context_When_executing_projection_query_Then_it_should_return_Null)).Options;

            var context = new ProjectionDbContext(options);
            var query = new FindProjectionQuery(context);
            
            var result = await query.Execute("testName", "projector", "testEvent");
            Assert.Null(result);
        }
    }
}