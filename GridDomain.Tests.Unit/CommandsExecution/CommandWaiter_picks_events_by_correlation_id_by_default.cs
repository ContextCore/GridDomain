using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class CommandWaiter_picks_events_by_correlation_id_by_default : NodeTestKit
    {
        public CommandWaiter_picks_events_by_correlation_id_by_default(ITestOutputHelper output) : this(new NodeTestFixture(output)) {}
        protected CommandWaiter_picks_events_by_correlation_id_by_default(NodeTestFixture fixture) : base(fixture.Add(new BalloonDomainConfiguration())) {}

        [Fact]
        public async Task Default_wait_for_event_matches_by_correlation_id_when_projection_builder_cares_about_metadata()
        {
            var commandA = new PlanTitleWriteCommand(10, Guid.NewGuid().ToString());
            var commandB = new PlanTitleWriteCommand(10, Guid.NewGuid().ToString());

            await Node.Execute(commandB);
            var res = await Node.Prepare(commandA)
                                .Expect<BalloonTitleChangedNotification>()
                                .Execute(false);

            //will pick right command by correlation Id
            Assert.Equal(commandA.AggregateId, res.Message<BalloonTitleChangedNotification>().BallonId);
        }
    }
}