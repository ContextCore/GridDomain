using System;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.AsyncAggregates
{
    [TestFixture]
    public class Wait_execution_of_async_method_until_projection_build_event_wait_by_caller: SampleDomainCommandExecutionTests
    {

        public Wait_execution_of_async_method_until_projection_build_event_wait_by_caller():base(true)
        {
            
        }
        [Then]
        public void Then_events_are_applied_to_aggregate_after_wait_finish()
        {
            var syncCommand = new AsyncMethodCommand(42, Guid.NewGuid());
            var expectedMessage = ExpectedMessage.Once<AggregateChangedEventNotification>(e => e.AggregateId,syncCommand.AggregateId);
            var task = GridNode.Execute<AggregateChangedEventNotification>(syncCommand, expectedMessage);
            if (!task.Wait(Timeout))
                throw new TimeoutException();

            var changedEvent = task.Result;
            Assert.AreEqual(syncCommand.AggregateId, changedEvent.AggregateId);
            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }
    }
}