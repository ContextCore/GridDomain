using System;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using NUnit.Framework;

namespace GridDomain.Tests.SynchroniousCommandExecute
{
    [TestFixture]
    public class InMemory_When_SyncExecute_until_aggregate_event_wait_by_Node_with_message_base_class : InMemorySampleDomainTests
    {
        [Then]
        public void SyncExecute_will_wait_for_any_one_of_expected_message_by_Node_with_message_base_class()
        {
            var syncCommand = new LongOperationCommand(42, Guid.NewGuid());

            var changeExpect = ExpectedMessage.Once<AggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            var createExpect = ExpectedMessage.Once<AggregateCreatedEvent>(e => e.SourceId, syncCommand.AggregateId);

            var result = GridNode.Execute<DomainEvent>(syncCommand, Timeout, changeExpect, createExpect);

            var aggregate = LoadAggregate<SampleAggregate>(syncCommand.AggregateId);
            Assert.AreEqual(syncCommand.Parameter.ToString(), aggregate.Value);
        }
    }
}