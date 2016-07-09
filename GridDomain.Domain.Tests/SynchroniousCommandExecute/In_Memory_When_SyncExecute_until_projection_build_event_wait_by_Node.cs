using System;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using NUnit.Framework;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    [TestFixture]
    public class In_Memory_When_SyncExecute_until_projection_build_event_wait_by_Node : SynchroniousCommandExecutionTests
    {
        public In_Memory_When_SyncExecute_until_projection_build_event_wait_by_Node(bool inMemory=true) : base(inMemory)
        {


        }

        public In_Memory_When_SyncExecute_until_projection_build_event_wait_by_Node():base(true)
        {
            
        }
        [Then]
        public void After_wait_aggregate_should_be_changed()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());
            GridNode.Execute(syncCommand,
                Timeout,
                ExpectedMessage.Once<AggregateChangedEventNotification>(e => e.AggregateId,
                    syncCommand.AggregateId)
                );

            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }
    }
}