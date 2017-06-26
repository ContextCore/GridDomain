using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class When_execute_command_Then_produced_event_are_available_for_projection_builders :
        SampleDomainCommandExecutionTests
    {
        public When_execute_command_Then_produced_event_are_available_for_projection_builders(ITestOutputHelper output)
            : base(output) {}

        [Fact]
        public async Task Async_method_should_produce_messages_for_projection_builders()
        {
            var cmd = new PlanTitleChangeCommand(42, Guid.NewGuid());

            await Node.Prepare(cmd).Expect<BalloonTitleChangedNotification>().Execute();
        }

        [Fact]
        public async Task Sync_method_should_produce_messages_for_projection_builders()
        {
            var cmd = new PlanTitleWriteCommand(42, Guid.NewGuid());

            await Node.Prepare(cmd).Expect<BalloonTitleChangedNotification>().Execute();
        }
    }
}