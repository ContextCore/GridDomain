using System;
using System.Threading.Tasks;
using GridDomain.EventHandlers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GridDomain.Projections.EntityFrameworkCore.Tests
{
    public class ProjectorTests
    {
        [Fact]
        public async Task Given_no_projection_When_projecting_Then_projection_entry_is_created()
        {
            var options = new DbContextOptionsBuilder<ProjectionDbContext>()
                .UseInMemoryDatabase(nameof(Given_no_projection_When_projecting_Then_projection_entry_is_created)).Options;
            
            
            var projector = new TestProjector("testProjector",()=>new ProjectionDbContext(options));

            await projector.Handle(new[] {new Sequenced<TestMessage>(new TestMessage("a",10), 1)});
            await projector.Handle(new[] {new Sequenced<TestMessage>(new TestMessage("a",20), 10)});


            using var context = new ProjectionDbContext(options);
            var projection = await context.Projections.FirstAsync();
                
            Assert.Equal(10, projection.Sequence);
            Assert.Equal("sequenced", projection.Event);
            Assert.Equal(nameof(TestProjector), projection.Projector);
            Assert.Equal("testProjection", projection.Name);
        }
        
        [Fact]
        public async Task Given_projection_When_projecting_Then_projection_entry_is_updated()
        {
            var options = new DbContextOptionsBuilder<ProjectionDbContext>()
                .UseInMemoryDatabase(nameof(Given_no_projection_When_projecting_Then_projection_entry_is_created)).Options;
            
            using var contextInit = new ProjectionDbContext(options);
            contextInit.Messages.Add(new TestMessage("a",10));
            contextInit.Projections.Add(new Projection(){Event="sequenced",Name="testProjection",Projector = nameof(TestProjector),Sequence = 5});
            
            var projector = new TestProjector("testProjector",()=>new ProjectionDbContext(options));

            await projector.Handle(new[] {new Sequenced<TestMessage>(new TestMessage("a",10), 6)});
            await projector.Handle(new[] {new Sequenced<TestMessage>(new TestMessage("a",20), 7)});


            using var context = new ProjectionDbContext(options);
            var projection = await context.Projections.FirstAsync();
                
            Assert.Equal(7, projection.Sequence);
            Assert.Equal("sequenced", projection.Event);
            Assert.Equal(nameof(TestProjector), projection.Projector);
            Assert.Equal("testProjection", projection.Name);
        }

        class ProjectionDbContext:DbContext,IProjectionDbContext
        {
            public ProjectionDbContext(DbContextOptions option):base(option)
            {
        
            }
             
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Projection>().HasKey(p => new {p.Name, p.Projector, p.Event});
                modelBuilder.Entity<TestMessage>().HasKey(p => p.Id);
            }
            
            public DbSet<Projection> Projections { get; set; }
            public DbSet<TestMessage> Messages { get; set; }
        }
        
        
        class TestProjector : IEventHandler<TestMessage>
        {
            private readonly Func<ProjectionDbContext> _contextFactory;

            public TestProjector(string projectorName, Func<ProjectionDbContext> contextFactory)
            {
                _contextFactory = contextFactory;
            }


            public async Task Handle(Sequenced<TestMessage>[] evt)
            {
                using var context = _contextFactory();
                var syncProjectionCommand = new SyncProjectionCommand(nameof(TestProjector),context);
                foreach (var m in evt)
                {
                    var existing = await context.Messages.FindAsync(m.Message.Id);
                    if (existing == null)
                        context.Messages.Add(m.Message);
                    else
                        existing.Value = m.Message.Value;

                    await syncProjectionCommand.Execute("testProjection", "sequenced", m.Sequence);
                }

                await context.SaveChangesAsync();
            }
        }
    
        public class TestMessage
        {
            public TestMessage(string id, int value)
            {
                Value = value;
                Id = id;
            }

            public int Value { get; set; }
            public string Id { get; set; }
        }

    }
}