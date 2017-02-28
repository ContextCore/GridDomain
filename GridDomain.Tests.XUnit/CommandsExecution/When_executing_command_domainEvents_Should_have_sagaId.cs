using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.XUnit.SampleDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
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
            var externalCallCommand = new AsyncMethodCommand(43, Guid.NewGuid(), Guid.NewGuid());

            var waitResults = await Node.Prepare(externalCallCommand)
                                        .Expect<SampleAggregateChangedEvent>()
                                        .Execute();

            var domainEvent = waitResults.Message<SampleAggregateChangedEvent>();

            Assert.Equal(externalCallCommand.SagaId, domainEvent.SagaId);
        }
    }
}