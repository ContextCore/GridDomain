using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class CommandWaiter_picks_events_by_correlation_id_by_default : BalloonDomainCommandExecutionTests
    {
        public CommandWaiter_picks_events_by_correlation_id_by_default(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task Default_wait_for_event_matches_by_correlation_id_when_projection_builder_cares_about_metadata()
        {
            var commandA = new PlanTitleWriteCommand(10, Guid.NewGuid());
            var commandB = new PlanTitleWriteCommand(10, Guid.NewGuid());

            Node.Execute(commandB);
            var res = await Node.Prepare(commandA)
                                .Expect<BalloonTitleChangedNotification>()
                                .Execute(false);

            //will pick right command by correlation Id
            Assert.Equal(commandA.AggregateId, res.Message<BalloonTitleChangedNotification>().BallonId);
        }
    }
}