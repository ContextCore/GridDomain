using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.AsyncAggregates
{
    public class When_WaitExecute_of_async_aggregate_with_fault_wait_by_Node : InMemorySampleDomainTests
    {
        [Then]
        public async Task Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            var syncCommand = new AlwaysFaultAsyncCommand(Guid.NewGuid());
            var expectedMessage = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            await GridNode.Execute(CommandPlan.New(syncCommand, Timeout, expectedMessage))
                          .ShouldThrow<SampleAggregateException>(e => e.StackTrace.Contains(typeof(SampleAggregate).Name));
        }
    }
}