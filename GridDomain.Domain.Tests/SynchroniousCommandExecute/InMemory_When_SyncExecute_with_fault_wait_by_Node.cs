using System;
using GridDomain.Node;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using NUnit.Framework;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    [TestFixture]
    public class InMemory_When_SyncExecute_with_fault_wait_by_Node : SampleDomainCommandExecutionTests
    {

        public InMemory_When_SyncExecute_with_fault_wait_by_Node() : base(true)
        {

        }
        public InMemory_When_SyncExecute_with_fault_wait_by_Node(bool inMemory) : base(inMemory)
        {


        }

        [Then]
        public void SyncExecute_throws_exception_from_aggregate_on_fault_wait_by_Node()
        {
            var syncCommand = new AlwaysFaultCommand(Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEvent>(e => e.SourceId,syncCommand.AggregateId);

            Assert.Throws<SampleAggregateException>(() => 
                        GridNode.Execute<AggregateChangedEvent>(syncCommand,Timeout,expectedMessage));
        }

     
    }
}