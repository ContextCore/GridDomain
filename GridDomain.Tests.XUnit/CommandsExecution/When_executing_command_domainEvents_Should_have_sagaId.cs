using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit.BalloonDomain.Commands;
using GridDomain.Tests.XUnit.BalloonDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.CommandsExecution
{
    public class When_executing_command_domainEvents_Should_have_sagaId : SampleDomainCommandExecutionTests
    {
        public When_executing_command_domainEvents_Should_have_sagaId(ITestOutputHelper output) : base(output) {}

        [Fact]
        public async Task When_async_method_finished_produced_events_has_sagaId_from_command()
        {
            var externalCallCommand = new PlanTitleChangeCommand(43, Guid.NewGuid(), Guid.NewGuid());

            var waitResults = await Node.Prepare(externalCallCommand).Expect<BalloonTitleChanged>().Execute();

            var domainEvent = waitResults.Message<BalloonTitleChanged>();

            Assert.Equal(externalCallCommand.SagaId, domainEvent.SagaId);
        }
    }
}