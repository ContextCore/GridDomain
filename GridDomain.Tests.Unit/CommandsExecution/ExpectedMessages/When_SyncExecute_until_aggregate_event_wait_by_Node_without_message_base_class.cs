using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.CommandsExecution.ExpectedMessages
{
    [TestFixture]
    public class When_SyncExecute_until_aggregate_event_wait_by_Node_without_message_base_class : InMemorySampleDomainTests
    {
        [Then]
        public async Task SyncExecute_will_finish()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());

            var changeExpect = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
           // var createExpect = Expect.Message<SampleAggregateCreatedEvent>(e => e.SourceId, syncCommand.AggregateId);

            await GridNode.Execute(CommandPlan.New(syncCommand, Timeout, changeExpect));

            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }

    }
}