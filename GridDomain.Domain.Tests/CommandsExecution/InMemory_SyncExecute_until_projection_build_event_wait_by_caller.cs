using System;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.CommandsExecution
{
    [TestFixture]
    public class InMemory_SyncExecute_until_projection_build_event_wait_by_caller: SampleDomainCommandExecutionTests
    {
        private AggregateChangedEventNotification _changedEvent;
        private SampleAggregate _aggregate;
        private LongOperationCommand _syncCommand;

        public InMemory_SyncExecute_until_projection_build_event_wait_by_caller():base(true)
        {
            
        }
        public InMemory_SyncExecute_until_projection_build_event_wait_by_caller(bool inMemory=true):base(inMemory)
        {
            
        }

        [TestFixtureSetUp]
        public void SyncExecute_until_projection_build_event_wait_by_caller()
        {
            _syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEventNotification>(e => e.AggregateId,_syncCommand.AggregateId);
            _changedEvent = GridNode.Execute<AggregateChangedEventNotification>(new CommandPlan( _syncCommand,expectedMessage), Timeout);
            _aggregate = LoadAggregate<SampleAggregate>(_syncCommand.AggregateId);
        }

        [Test]
        public void Emmited_event_has_correct_id()
        {
            Assert.AreEqual(_syncCommand.AggregateId, _changedEvent.AggregateId);

        }

        [Test]
        public void Aggregate_has_correct_state_from_command()
        {
            Assert.AreEqual(_syncCommand.Parameter.ToString(), _aggregate.Value);

        }
    }
}