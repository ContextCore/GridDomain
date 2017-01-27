using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
  
    public class Execute_command_until_projection_build_notification_fetching_results: SampleDomainCommandExecutionTests
    {
        private AggregateChangedEventNotification _changedEvent;
        private SampleAggregate _aggregate;
        private LongOperationCommand _syncCommand;
        private IWaitResults _results;


        [OneTimeSetUp]
        public async Task Given_command_executes_with_waiter_When_fetching_results()
        {
            _syncCommand = new LongOperationCommand(1000, Guid.NewGuid());


           _results = await Node.Prepare(_syncCommand)
                                    .Expect<AggregateChangedEventNotification>(e => e.AggregateId == _syncCommand.AggregateId)
                                    .Execute(DefaultTimeout);

            _changedEvent = _results.Message<AggregateChangedEventNotification>();

            _aggregate = this.LoadAggregate<SampleAggregate>(_syncCommand.AggregateId);
        }

       [Fact]
        public void Results_contains_received_messages()
        {
            CollectionAssert.IsNotEmpty(_results.All);
        }

       [Fact]
        public void Results_contains_requested_message()
        {
            Assert.NotNull(_changedEvent);
        }

       [Fact]
        public void Emmited_event_has_correct_id()
        {
            Assert.Equal(_syncCommand.AggregateId, _changedEvent?.AggregateId);
        }

       [Fact]
        public void Aggregate_has_correct_state_from_command()
        {
            Assert.Equal(_syncCommand.Parameter.ToString(), _aggregate.Value);
        }


    }
}