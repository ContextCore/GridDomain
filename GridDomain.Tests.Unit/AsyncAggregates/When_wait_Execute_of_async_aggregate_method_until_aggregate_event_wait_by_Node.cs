using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.AsyncAggregates
{
    [TestFixture]
    public class When_wait_Execute_of_async_aggregate_method_until_aggregate_event_wait_by_Node : SampleDomainCommandExecutionTests
    {

        public When_wait_Execute_of_async_aggregate_method_until_aggregate_event_wait_by_Node():base(true)
        {
            
        }

        [Then]
        public async Task After_wait_end_aggregate_event_is_applied()
        {
            var syncCommand = new AsyncMethodCommand(42, Guid.NewGuid());
            await GridNode.Execute(CommandPlan.New(syncCommand, 
                                                   Timeout, 
                                                   Expect.Message<SampleAggregateChangedEvent>(e=>e.SourceId,syncCommand.AggregateId)));

            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }
    }
}