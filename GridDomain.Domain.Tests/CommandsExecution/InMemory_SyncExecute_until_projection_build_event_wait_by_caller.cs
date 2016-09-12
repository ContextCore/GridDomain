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

        public InMemory_SyncExecute_until_projection_build_event_wait_by_caller():base(true)
        {
            
        }
        public InMemory_SyncExecute_until_projection_build_event_wait_by_caller(bool inMemory=true):base(inMemory)
        {
            
        }
        [Then]
        public void SyncExecute_until_projection_build_event_wait_by_caller()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEventNotification>(e => e.AggregateId,syncCommand.AggregateId);
            var task = GridNode.Execute<AggregateChangedEventNotification>(syncCommand, expectedMessage);
            if (!task.Wait(Timeout))
                throw new TimeoutException();

            var changedEvent = task.Result;
            Assert.AreEqual(syncCommand.AggregateId, changedEvent.AggregateId);
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }
    }
}