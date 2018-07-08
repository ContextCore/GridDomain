using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.BalloonDomain.Commands;
using GridDomain.Tests.Unit.BalloonDomain.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.CommandsExecution.ExecutionWithErrors;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.CommandsExecution
{
    public class When_executing_command_domainEvents_Should_have_processId : NodeTestKit
    {
        public When_executing_command_domainEvents_Should_have_processId(ITestOutputHelper output) : this(new NodeTestFixture(output)) {}
        protected When_executing_command_domainEvents_Should_have_processId(NodeTestFixture fixture) : base(fixture.Add(new BalloonDomainConfiguration())) {}

        [Fact]
        public async Task When_async_method_finished_produced_events_has_processId_from_command()
        {
            var externalCallCommand = new PlanTitleChangeCommand(Guid.NewGuid().ToString(), 43);

            var waitResults = await Node.Prepare(externalCallCommand).Expect<BalloonTitleChanged>().Execute();

            var domainEvent = waitResults.Message<BalloonTitleChanged>();

            Assert.Equal(externalCallCommand.ProcessId, domainEvent.ProcessId);
        }
    }
}