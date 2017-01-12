using System;
using System.Threading.Tasks;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.CommandsExecution
{

    [TestFixture]
    public class When_executing_command_domainEvents_Should_have_sagaId : InMemorySampleDomainTests
    {
        [Test]
        public async Task When_async_method_finished_produced_events_has_sagaId_from_command()
        {
            var externalCallCommand = new AsyncMethodCommand(43, Guid.NewGuid(), Guid.NewGuid());

            var waitResults = await GridNode.PrepareCommand(externalCallCommand).Expect<SampleAggregateChangedEvent>().Execute();
            var domainEvent = waitResults.Message<SampleAggregateChangedEvent>();

            Assert.AreEqual(externalCallCommand.SagaId, domainEvent.SagaId);
        }
    }
}
