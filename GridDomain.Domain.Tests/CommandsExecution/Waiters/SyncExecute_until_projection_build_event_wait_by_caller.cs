using System;
using GridDomain.CQRS;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.CommandsExecution.Waiters
{
    [TestFixture]
    public class SyncExecute_until_projection_build_event_wait_by_caller: SampleDomainCommandExecutionTests
    {
        private AggregateChangedEventNotification _changedEvent;
        private SampleAggregate _aggregate;
        private LongOperationCommand _syncCommand;

        public SyncExecute_until_projection_build_event_wait_by_caller():base(true)
        {
            
        }
        public SyncExecute_until_projection_build_event_wait_by_caller(bool inMemory=true):base(inMemory)
        {
            
        }

        [OneTimeSetUp]
        public void Given_SyncExecute_until_projection_build_event_wait_by_caller()
        {
            _syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId,_syncCommand.AggregateId);

            _changedEvent = GridNode.Execute(CommandPlan.New(_syncCommand, Timeout, expectedMessage)).Result;

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