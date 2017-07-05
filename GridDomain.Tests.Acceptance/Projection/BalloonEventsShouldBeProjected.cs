using System;
using System.Configuration;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Acceptance.BalloonDomain;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Acceptance.Projection
{
    public static class ConnectionStrings
    {
        static ConnectionStrings()
        {
            AutoTestDb = ConfigurationManager.ConnectionStrings["AutoTestDb"]?.ConnectionString 
                ?? 
                new AutoTestLocalDbConfiguration().ReadModelConnectionString;
        }

        public static string AutoTestDb { get; }
    }

    public class BalloonEventsShouldBeProjected : NodeTestKit
    {
        public BalloonEventsShouldBeProjected(ITestOutputHelper output) :
            base(output, new BalloonWithProjectionFixture() {InMemory = false}) {}

        [Fact]
        public async Task When_Executing_command_events_should_be_projected()
        {
            var autoTestDb = ConnectionStrings.AutoTestDb;

            var dbOptions = new DbContextOptionsBuilder<BalloonContext>().UseSqlServer(autoTestDb).Options;
            //warm up EF 
            using (var context = new BalloonContext(dbOptions))
            {
                context.BalloonCatalog.Add(new BalloonCatalogItem() {BalloonId = Guid.NewGuid(),LastChanged = DateTime.UtcNow,Title="WarmUp"});
                await context.SaveChangesAsync();
            }

            await TestDbTools.Truncate(autoTestDb, "BalloonCatalog");
            
            var cmd = new InflateNewBallonCommand(123, Guid.NewGuid());
            await Node.Prepare(cmd)
                      .Expect<BalloonCreatedNotification>()
                      .Execute(TimeSpan.FromSeconds(30));

            using (var context = new BalloonContext(dbOptions))
            {
                var catalogItem = await context.BalloonCatalog.FindAsync(cmd.AggregateId);
                Assert.Equal(cmd.Title.ToString(), catalogItem.Title);
            }
        }
    }
}