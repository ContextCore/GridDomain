using System;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    [TestFixture]
    public class InMemory_When_SyncExecute_until_aggregate_event_wait_by_Node_without_message_base_class : InMemorySampleDomainTests
    {
        [Then]
        public void SyncExecute_will_finish()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());

            var changeExpect = ExpectedMessage.Once<AggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            var createExpect = ExpectedMessage.Once<AggregateCreatedEvent>(e => e.SourceId, syncCommand.AggregateId);

            var result = GridNode.Execute(syncCommand, Timeout, changeExpect, createExpect);

            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }
    }
}