using System;
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
        public void Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            var syncCommand = new AlwaysFaultCommand(Guid.NewGuid());
            var expectedMessage = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            string stackTraceString = "";
            try
            {
                GridNode.ExecuteSync(syncCommand, Timeout, expectedMessage);
            }
            catch (SampleAggregateException ex)
            {
                stackTraceString = ex.StackTrace;
            }

            Assert.True(stackTraceString.Contains(typeof(SampleAggregate).Name));
        }
    }
}