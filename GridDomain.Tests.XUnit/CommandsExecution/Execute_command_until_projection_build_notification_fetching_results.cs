using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit.BalloonDomain;
using GridDomain.Tests.XUnit.BalloonDomain.Commands;
using GridDomain.Tests.XUnit.BalloonDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class Execute_command_until_projection_build_notification_fetching_results : SampleDomainCommandExecutionTests
    {
        public Execute_command_until_projection_build_notification_fetching_results(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task Given_command_executes_with_waiter_When_fetching_results()
        {
            var syncCommand = new PlanTitleWriteCommand(1000, Guid.NewGuid());

            var results = await Node.Prepare(syncCommand)
                                    .Expect<AggregateChangedEventNotification>()
                                    .Execute();

            var changedEvent = results.Message<AggregateChangedEventNotification>();
            var aggregate = await this.LoadAggregate<Balloon>(syncCommand.AggregateId);

            //Results_contains_received_messages()
            Assert.NotEmpty(results.All);
            //Results_contains_requested_message()
            Assert.NotNull(changedEvent);
            //Emmited_event_has_correct_id()
            Assert.Equal(syncCommand.AggregateId, changedEvent?.AggregateId);
            //Aggregate_has_correct_state_from_command()
            Assert.Equal(syncCommand.Parameter.ToString(), aggregate.Title);
        }
    }
}