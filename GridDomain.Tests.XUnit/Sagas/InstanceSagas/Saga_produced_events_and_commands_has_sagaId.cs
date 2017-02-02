using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.InstanceSagas
{
   
    public class Saga_produced_events_and_commands_has_sagaId : SoftwareProgrammingInstanceSagaTest
    {
        public Saga_produced_events_and_commands_has_sagaId() : base(true)
        {

        }

        public Saga_produced_events_and_commands_has_sagaId(bool inMemory) : base(inMemory)
        {

        }

        [Fact]
        public void When_dispatch_command_than_command_should_have_right_sagaId()
        {
            var domainEvent = new GotTiredEvent(Guid.NewGuid());

            GridNode.Pipe.SagaProcessor.Tell(new Initialize(TestActor));
            GridNode.Pipe.SagaProcessor.Tell(new MessageMetadataEnvelop<DomainEvent[]>(new []{domainEvent},
                                                                                       MessageMetadata.Empty));

            var sagaCompleteMsg = FishForMessage<IMessageMetadataEnvelop<ICommand>>(m => true,TimeSpan.FromMinutes(10));
            var command = sagaCompleteMsg.Message;

            Assert.Equal(domainEvent.SagaId, command.SagaId);
            Assert.IsAssignableFrom<MakeCoffeCommand>(command);
        }

        [Fact]
        public async Task When_raise_saga_than_saga_created_event_should_have_right_sagaId()
        {
            var domainEvent = new GotTiredEvent(Guid.NewGuid(),Guid.NewGuid(),Guid.NewGuid());

            var waitResults = await GridNode.NewDebugWaiter()
                                            .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                                            .Create()
                                            .SendToSagas(domainEvent);

            Assert.Equal(domainEvent.SagaId, waitResults.Message<SagaCreatedEvent<SoftwareProgrammingSagaData>>().SagaId);
        }

        protected override TimeSpan DefaultTimeout => TimeSpan.FromSeconds(5);
    }
}