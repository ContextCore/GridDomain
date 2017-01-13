using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    [TestFixture]
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


           _results = await GridNode.PrepareCommand(_syncCommand)
                                    .Expect<AggregateChangedEventNotification>(e => e.AggregateId == _syncCommand.AggregateId)
                                    .Execute(Timeout);

            _changedEvent = _results.Message<AggregateChangedEventNotification>();

            _aggregate = LoadAggregate<SampleAggregate>(_syncCommand.AggregateId);
        }

        [Test]
        public void Results_contains_received_messages()
        {
            CollectionAssert.IsNotEmpty(_results.All);
        }

        [Test]
        public void Results_contains_requested_message()
        {
            Assert.NotNull(_changedEvent);
        }

        [Test]
        public void Emmited_event_has_correct_id()
        {
            Assert.AreEqual(_syncCommand.AggregateId, _changedEvent?.AggregateId);
        }

        [Test]
        public void Aggregate_has_correct_state_from_command()
        {
            Assert.AreEqual(_syncCommand.Parameter.ToString(), _aggregate.Value);
        }


    }
}