using System;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using NUnit.Framework;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    [TestFixture]
    public class InMemory_When_SyncExecute_until_aggregate_event_wait_by_caller : SynchroniousCommandExecutionTests
    {
        public InMemory_When_SyncExecute_until_aggregate_event_wait_by_caller():base(true)
        {
            
        }
        public InMemory_When_SyncExecute_until_aggregate_event_wait_by_caller(bool inMemory=true):base(inMemory)
        {
            
        }

        [Then]
        public void SyncExecute_until_aggregate_event_wait_by_caller()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEvent>(nameof(AggregateChangedEvent.SourceId),
                syncCommand.AggregateId);
            var task = GridNode.Execute(syncCommand,expectedMessage);
            if (!task.Wait(Timeout))
                throw new TimeoutException();

            //to finish persistence
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }
    }
}