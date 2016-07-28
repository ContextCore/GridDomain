using System;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.AsyncAggregates
{
    [TestFixture]
    public class When_wait_execution_of_async_method_until_aggregate_event_wait_by_caller : SampleDomainCommandExecutionTests
    {
        public When_wait_execution_of_async_method_until_aggregate_event_wait_by_caller():base(true)
        {
            
        }

        [Then]
        public void Then_events_are_applied_to_aggregate_after_wait_finish()
        {
            var syncCommand = new AsyncMethodCommand(42, Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<SampleAggregateChangedEvent>(nameof(SampleAggregateChangedEvent.SourceId),
                                                                              syncCommand.AggregateId);
            var task = GridNode.Execute(syncCommand,expectedMessage);
            if (!task.Wait(Timeout))
                throw new TimeoutException();

            //to finish persistence
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }
    }
}