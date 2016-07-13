using System;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using NUnit.Framework;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    [TestFixture]
    public class InMemory_when_sync_execute_with_command_fault : InMemorySampleDomainTests
    {

        [Then]
        public void Then_execute_throws_exception_from_aggregate_with_stack_trace()
        {
            var syncCommand = new AlwaysFaultCommand(Guid.NewGuid());
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