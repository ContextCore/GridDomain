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
    public class Sync_execute_with_command_fault : InMemorySampleDomainTests
    {
        [Then]
        public async Task Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            var syncCommand = new AlwaysFaultCommand(Guid.NewGuid());
            var expectedMessage = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);

            await GridNode.Execute(CommandPlan.New(syncCommand, Timeout, expectedMessage))
                           .ShouldThrow<SampleAggregateException>(
                                         e => e.StackTrace.Contains(typeof(SampleAggregate).Name));
         
        }
    }
}