using System;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.Tests.Unit.CommandsExecution;
using GridDomain.Tests.Unit.SampleDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.AsyncAggregates
{

    [TestFixture]
    public class When_async_method_is_called_domainEvents_are_persisted : InMemorySampleDomainTests
    {
        [Test]
        public async Task When_async_method_finished_produced_events_has_sagaId_from_command()
        {
            var externalCallCommand = new AsyncMethodCommand(43, Guid.NewGuid(), Guid.NewGuid());
            var domainEvent = await GridNode.Execute(CommandPlan.New(externalCallCommand, 
                                                     TimeSpan.FromDays(1), 
                                                     Expect.Message<SampleAggregateChangedEvent>(e=>e.SourceId,
                                                     externalCallCommand.AggregateId)));

            Assert.AreEqual(externalCallCommand.SagaId, domainEvent.SagaId);
        }
    }
}
