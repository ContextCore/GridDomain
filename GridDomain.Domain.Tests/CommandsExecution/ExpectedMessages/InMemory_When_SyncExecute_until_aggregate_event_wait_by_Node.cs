using System;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.CommandsExecution
{
    [TestFixture]
    public class SyncExecute_until_aggregate_event_wait_by_Node : SampleDomainCommandExecutionTests
    {

        public SyncExecute_until_aggregate_event_wait_by_Node():base(true)
        {
            
        }
        public SyncExecute_until_aggregate_event_wait_by_Node(bool inMemory=true) : base(inMemory)
        {


        }

        [Then]
        public void Then_SyncExecute_until_aggregate_event_wait_by_Node()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            GridNode.ExecuteSync(syncCommand,
                Timeout,
                Expect.Message<SampleAggregateChangedEvent>(nameof(SampleAggregateChangedEvent.SourceId),syncCommand.AggregateId));

            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }


    }
}