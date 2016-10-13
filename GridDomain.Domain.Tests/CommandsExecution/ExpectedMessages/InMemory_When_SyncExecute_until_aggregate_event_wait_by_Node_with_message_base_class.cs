using System;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.CommandsExecution
{
    [TestFixture]
    public class When_SyncExecute_until_aggregate_event_wait_by_Node_with_message_base_class : InMemorySampleDomainTests
    {
        [Then]
        public void SyncExecute_will_wait_for_all_of_expected_message_by_Node_with_message_base_class()
        {
            var syncCommand = new LongOperationCommand(1000, Guid.NewGuid());

            var changeExpect = Expect.Message<SampleAggregateChangedEvent>(e => e.SourceId, syncCommand.AggregateId);
            var createExpect = Expect.Message<SampleAggregateCreatedEvent>(e => e.SourceId, syncCommand.AggregateId);

            Assert.Throws<TimeoutException>(() =>
            {
                var commandPlan = new CommandPlan<object>(syncCommand,
                                                          TimeSpan.FromSeconds(1),
                                                          changeExpect,
                                                          createExpect);

                GridNode.ExecuteSync(commandPlan);
            });
        }
    }
}