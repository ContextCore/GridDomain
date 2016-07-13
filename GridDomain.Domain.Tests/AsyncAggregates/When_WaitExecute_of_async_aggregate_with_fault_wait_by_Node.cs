using System;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.AsyncAggregates
{
    public class When_WaitExecute_of_async_aggregate_with_fault_wait_by_Node : InMemorySampleDomainTests
    {
        [Then]
        public void Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            var syncCommand = new AlwaysFaultAsyncCommand(Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            string stackTraceString = "";
            try
            {
                GridNode.Execute<AggregateChangedEvent>(syncCommand, Timeout, expectedMessage);
            }
            catch (SampleAggregateException ex)
            {
                stackTraceString = ex.StackTrace;
            }

            Assert.True(stackTraceString.Contains(typeof(SampleAggregate).Name));
        }
    }
}