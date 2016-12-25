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
    public class When_WaitExecute_of_async_aggregate_method_with_one_event_fault_wait_by_Node : SampleDomainCommandExecutionTests
    {
        public When_WaitExecute_of_async_aggregate_method_with_one_event_fault_wait_by_Node() : base(true)
        {

        }

        [Then]
        public async Task Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            var syncCommand = new AsyncFaultWithOneEventCommand(42, Guid.NewGuid(), Guid.NewGuid(),TimeSpan.FromMilliseconds(500));
            var expectedMessage = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            await GridNode.Execute(CommandPlan.New(syncCommand, Timeout, expectedMessage))
                          .ShouldThrow<SampleAggregateException>(ex => ex.StackTrace.Contains(typeof(SampleAggregate).Name));
        }

    }
}