using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.Tests.Acceptance.XUnit.BalloonDomain;
using GridDomain.Tests.XUnit;
using GridDomain.Tests.XUnit.BalloonDomain.Commands;
using GridDomain.Tests.XUnit.BalloonDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;
using GridDomain.CQRS;
using GridDomain.Tests.Framework;

namespace GridDomain.Tests.Acceptance.XUnit.Projection
{
    public class BalloonEventsShouldBeProjected : NodeTestKit
    {
        public BalloonEventsShouldBeProjected(ITestOutputHelper output) :
            base(output, new BalloonWithProjectionFixture() {InMemory = false}) {}

        [Fact]
        public async Task When_Executing_command_events_should_be_projected()
        {
            //warm up EF 
            using (var context = new BalloonContext(Fixture.AkkaConfig.Persistence.JournalConnectionString))
            {
                context.BalloonCatalog.Add(new BalloonCatalogItem() {BalloonId = Guid.NewGuid(),LastChanged = DateTime.UtcNow,Title="WarmUp"});
                await context.SaveChangesAsync();
            }

            await TestDbTools.Truncate(Fixture.AkkaConfig.Persistence.JournalConnectionString, "BalloonCatalogItems");
            
            var cmd = new InflateNewBallonCommand(123, Guid.NewGuid());
            await Node.Prepare(cmd)
                      .Expect<BalloonCreatedNotification>()
                      .Execute(TimeSpan.FromSeconds(30));

            using (var context = new BalloonContext(Fixture.AkkaConfig.Persistence.JournalConnectionString))
            {
                var catalogItem = await context.BalloonCatalog.FindAsync(cmd.AggregateId);
                Assert.Equal(cmd.Title.ToString(), catalogItem.Title);
            }
        }
    }
}