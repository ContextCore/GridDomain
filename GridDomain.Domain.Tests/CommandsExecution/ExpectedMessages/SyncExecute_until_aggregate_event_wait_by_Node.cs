using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.CommandsExecution.ExpectedMessages
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
        public async Task Then_SyncExecute_until_aggregate_event_wait_by_Node()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());
            await GridNode.Execute(CommandPlan.New(syncCommand, TimeSpan.FromDays(1), Expect.Message<SampleAggregateChangedEvent>(nameof(SampleAggregateChangedEvent.SourceId),syncCommand.AggregateId)));

            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }


    }
}