using System;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using NUnit.Framework;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    [TestFixture]
    public class InMemory_When_SyncExecute_with_fault_wait_by_Node : SynchroniousCommandExecutionTests
    {

        public InMemory_When_SyncExecute_with_fault_wait_by_Node() : base(true)
        {

        }
        public InMemory_When_SyncExecute_with_fault_wait_by_Node(bool inMemory = true) : base(inMemory)
        {


        }

        [Then]
        public void SyncExecute_until_aggregate_event_wait_by_Node()
        {
            var syncCommand = new AlwaysFaultCommand(Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEvent>(e => e.SourceId,syncCommand.AggregateId);

            var result = GridNode.Execute<AggregateChangedEvent>(syncCommand,Timeout,expectedMessage);

            //var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            //Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }
    }
}