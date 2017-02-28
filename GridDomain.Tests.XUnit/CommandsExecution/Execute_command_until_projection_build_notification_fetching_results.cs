using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit.SampleDomain;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class Execute_command_until_projection_build_notification_fetching_results : SampleDomainCommandExecutionTests
    {
        public Execute_command_until_projection_build_notification_fetching_results(ITestOutputHelper output) : base(output) {}

        private AggregateChangedEventNotification _changedEvent;
        private SampleAggregate _aggregate;
        private LongOperationCommand _syncCommand;
        private IWaitResults _results;

        [Fact]
        public async Task Given_command_executes_with_waiter_When_fetching_results()
        {
            _syncCommand = new LongOperationCommand(1000, Guid.NewGuid());


            _results =
                await
                    Node.Prepare(_syncCommand)
                        .Expect<AggregateChangedEventNotification>(e => e.AggregateId == _syncCommand.AggregateId)
                        .Execute();

            _changedEvent = _results.Message<AggregateChangedEventNotification>();
            _aggregate = await this.LoadAggregate<SampleAggregate>(_syncCommand.AggregateId);
            //Results_contains_received_messages()
            Assert.NotEmpty(_results.All);
            //Results_contains_requested_message()
            Assert.NotNull(_changedEvent);
            //Emmited_event_has_correct_id()
            Assert.Equal(_syncCommand.AggregateId, _changedEvent?.AggregateId);
            //Aggregate_has_correct_state_from_command()
            Assert.Equal(_syncCommand.Parameter.ToString(), _aggregate.Value);
        }
    }
}