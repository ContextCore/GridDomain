using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Sagas
{
    public class Saga_produced_events_and_commands_has_sagaId : SoftwareProgrammingSagaTest
    {
        public Saga_produced_events_and_commands_has_sagaId(ITestOutputHelper helper) : base(helper) {}

        [Fact]
        public async Task When_dispatch_command_than_command_should_have_right_sagaId()
        {
            Node.Pipe.SagaProcessor.Tell(new Initialize(TestActor));

            var sagaCreatedMsg = await Node.NewDebugWaiter()
                                           .Expect<SagaCreated<SoftwareProgrammingState>>()
                                           .Create()
                                           .SendToSagas(new GotTiredEvent(Guid.NewGuid()));

            var sagaCompleteMsg = FishForMessage<IMessageMetadataEnvelop<ICommand>>(m => true,TimeSpan.FromHours(1));
            var command = sagaCompleteMsg.Message;

            Assert.Equal(sagaCreatedMsg.Message<SagaStateEvent>().SourceId, command.SagaId);
            Assert.IsAssignableFrom<MakeCoffeCommand>(command);
        }

        [Fact]
        public async Task When_saga_created_from_event_with_sagaId_new_Id_is_generated()
        {
            var domainEvent = new GotTiredEvent(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            var waitResults = await Node.NewDebugWaiter()
                                        .Expect<SagaCreated<SoftwareProgrammingState>>()
                                        .Create()
                                        .SendToSagas(domainEvent);

            Assert.NotEqual(domainEvent.SagaId, waitResults.Message<SagaCreated<SoftwareProgrammingState>>().State.Id);
        }
    }
}