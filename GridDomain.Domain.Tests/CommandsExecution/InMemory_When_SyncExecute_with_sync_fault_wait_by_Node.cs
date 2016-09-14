using System;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.CommandsExecution
{
    [TestFixture]
    public class InMemory_When_SyncExecute_with_sync_fault_wait_by_Node : SampleDomainCommandExecutionTests
    {

        public InMemory_When_SyncExecute_with_sync_fault_wait_by_Node() : base(true)
        {

        }
        public InMemory_When_SyncExecute_with_sync_fault_wait_by_Node(bool inMemory) : base(inMemory)
        {


        }

        [Then]
        public void SyncExecute_throws_exception_from_aggregate_on_fault_wait_by_Node()
        {
            var syncCommand = new AlwaysFaultCommand(Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<SampleAggregateChangedEvent>(e => e.SourceId,syncCommand.AggregateId);

            Assert.Throws<SampleAggregateException>(() => 
                        GridNode.Execute<SampleAggregateChangedEvent>(syncCommand,Timeout,expectedMessage));
        }

    }
}