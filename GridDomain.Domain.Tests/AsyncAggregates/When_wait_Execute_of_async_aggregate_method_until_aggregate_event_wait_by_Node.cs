using System;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SynchroniousCommandExecute;
using NUnit.Framework;

namespace GridDomain.Tests.AsyncAggregates
{
    [TestFixture]
    public class When_wait_Execute_of_async_aggregate_method_until_aggregate_event_wait_by_Node : SampleDomainCommandExecutionTests
    {

        public When_wait_Execute_of_async_aggregate_method_until_aggregate_event_wait_by_Node():base(true)
        {
            
        }

        [Then]
        public void After_wait_end_aggregate_event_is_applied()
        {
            var syncCommand = new AsyncMethodCommand(42, Guid.NewGuid());
            GridNode.Execute(syncCommand,
                Timeout,
                ExpectedMessage.Once<AggregateChangedEvent>(nameof(AggregateChangedEvent.SourceId),syncCommand.AggregateId));

            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }
    }
}