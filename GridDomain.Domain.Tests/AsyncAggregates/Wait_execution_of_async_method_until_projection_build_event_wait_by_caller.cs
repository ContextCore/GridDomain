using System;
using GridDomain.CQRS;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using NUnit.Framework;
using AggregateChangedEventNotification = GridDomain.Tests.SampleDomain.ProjectionBuilders.AggregateChangedEventNotification;

namespace GridDomain.Tests.AsyncAggregates
{
    [TestFixture]
    public class Wait_execution_of_async_method_until_projection_build_event_wait_by_caller: InMemorySampleDomainTests
    {

        [Then]
        public void Then_events_are_applied_to_aggregate_after_wait_finish()
        {
            var cmd = new AsyncMethodCommand(42, Guid.NewGuid());
            var expectedMessage = Expect.Message<AggregateChangedEventNotification>(e => e.AggregateId, cmd.AggregateId);
            var task = GridNode.Execute<AggregateChangedEventNotification>(cmd, expectedMessage);

            if (!task.Wait(Timeout))
                throw new TimeoutException();

            var aggregate = LoadAggregate<SampleAggregate>(cmd.AggregateId);
            Assert.AreEqual(cmd.Parameter.ToString(), aggregate.Value);
        }
    }
}