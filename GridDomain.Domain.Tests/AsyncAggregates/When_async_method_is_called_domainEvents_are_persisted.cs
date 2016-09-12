using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS.Messaging;
using GridDomain.Node;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Quartz;
using GridDomain.Tests.CommandsExecution;
using GridDomain.Tests.FutureEvents;
using GridDomain.Tests.SampleDomain;
using GridDomain.Tests.SampleDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace GridDomain.Tests.AsyncAggregates
{

    [TestFixture]
    public class When_async_method_is_called_domainEvents_are_persisted : InMemorySampleDomainTests
    {
        [Test]
        public void When_async_method_finished_produced_events_has_sagaId_from_command()
        {
            var externalCallCommand = new AsyncMethodCommand(43, Guid.NewGuid(), Guid.NewGuid());
            var domainEvent = GridNode.Execute<SampleAggregateChangedEvent>(externalCallCommand, Timeout,
                                                    ExpectedMessage.Once<SampleAggregateChangedEvent>(nameof(SampleAggregateChangedEvent.SourceId),
                                                    externalCallCommand.AggregateId)
                                                    );

            Assert.AreEqual(externalCallCommand.SagaId, domainEvent.SagaId);
        }
    }
}
