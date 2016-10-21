using System;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.MessageWaiting.Commanding
{
    [TestFixture]
    public class Execute_command_until_projection_build_event_fetching_results: SampleDomainCommandExecutionTests
    {
        private AggregateChangedEventNotification _changedEvent;
        private SampleAggregate _aggregate;
        private LongOperationCommand _syncCommand;
        private IWaitResults _results;


        [OneTimeSetUp]
        public void Given_command_executes_with_waiter_When_fetching_results()
        {
            _syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,_syncCommand.AggregateId);



           _results = GridNode.NewCommandWaiter()
                                .Expect<AggregateChangedEventNotification>(e => e.AggregateId == _syncCommand.AggregateId)
                                .Create(Timeout)
                                .Execute(_syncCommand)
                                .Result;

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