using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.SampleDomain;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using NUnit.Framework;
using AggregateChangedEventNotification = GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders.AggregateChangedEventNotification;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    [TestFixture]
    public class When_execute_command_Then_produced_event_are_available_for_projection_builders: InMemorySampleDomainTests
    {

        [Then]
        public async Task Async_method_should_produce_messages_for_projection_builders()
        {
            var cmd = new AsyncMethodCommand(42, Guid.NewGuid());

            await GridNode.PrepareCommand(cmd)
                          .Expect<AggregateChangedEventNotification>()
                          .Execute();
        }

        [Then]
        public async Task Sync_method_should_produce_messages_for_projection_builders()
        {
            var cmd = new LongOperationCommand(42, Guid.NewGuid());

            await GridNode.PrepareCommand(cmd)
                          .Expect<AggregateChangedEventNotification>()
                          .Execute();
        }
    }
}