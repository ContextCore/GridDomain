using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.Common;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;
using Akka.Actor;
using GridDomain.CQRS;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    [TestFixture]
    public class Saga_produced_events_and_commands_has_sagaId : SoftwareProgrammingInstanceSagaTest
    {
        public Saga_produced_events_and_commands_has_sagaId() : base(true)
        {

        }

        public Saga_produced_events_and_commands_has_sagaId(bool inMemory) : base(inMemory)
        {

        }

        [Test]
        public void When_dispatch_command_than_command_should_have_right_sagaId()
        {
            var domainEvent = new GotTiredEvent(Guid.NewGuid());

            GridNode.Pipe.SagaProcessor.Tell(new Initialize(TestActor));
            GridNode.Pipe.SagaProcessor.Tell(new MessageMetadataEnvelop<DomainEvent[]>(new []{domainEvent},
                                                                                       MessageMetadata.Empty));

            var sagaCompleteMsg = FishForMessage<IMessageMetadataEnvelop<ICommand>>(m => true,TimeSpan.FromMinutes(10));
            var command = sagaCompleteMsg.Message;

            Assert.AreEqual(domainEvent.SagaId, command.SagaId);
            Assert.IsInstanceOf<MakeCoffeCommand>(command);
        }

        [Test]
        public async Task When_raise_saga_than_saga_created_event_should_have_right_sagaId()
        {
            var domainEvent = new GotTiredEvent(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid());

            var waitResults = await GridNode.NewDebugWaiter()
                                            .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                                            .Create()
                                            .SendToSaga(domainEvent);

            Assert.AreEqual(domainEvent.SagaId, waitResults.Message<SagaCreatedEvent<SoftwareProgrammingSagaData>>().SagaId);
        }

        protected override TimeSpan Timeout => TimeSpan.FromSeconds(5);
    }
}