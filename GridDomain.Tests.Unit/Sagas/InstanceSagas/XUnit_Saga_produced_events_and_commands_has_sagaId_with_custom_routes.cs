using System;
using System.Threading.Tasks;
using Akka.Actor;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas
{
    [TestFixture]
    public class XUnit_Saga_produced_events_and_commands_has_sagaId_with_custom_routes : ProgrammingSoftwareSagaTest_with_custom_routes
    {

        public XUnit_Saga_produced_events_and_commands_has_sagaId_with_custom_routes() : base(true)
        {

        }

        [Test]
        public void When_dispatch_command_than_command_should_have_right_sagaId()
        { 
            var domainEvent = new GotTiredEvent(Guid.NewGuid());

            GridNode.Pipe.SagaProcessor.Tell(new Initialize(TestActor));
            GridNode.Pipe.SagaProcessor.Tell(new MessageMetadataEnvelop<DomainEvent[]>(new[] { domainEvent },
                                                                                       MessageMetadata.Empty));

            var sagaCompleteMsg = FishForMessage<IMessageMetadataEnvelop<ICommand>>(m => true);
            var command = sagaCompleteMsg.Message;

            Assert.AreEqual(domainEvent.SagaId, command.SagaId);
            Assert.IsInstanceOf<MakeCoffeCommand>(command);
        }

        [Test]
        public async Task When_raise_saga_than_saga_created_event_should_have_right_sagaId()
        {
            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid());

            var waitResults = await GridNode.NewDebugWaiter()
                                            .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                                            .Create()
                                            .SendToSaga(gotTiredEvent);

            var expectedCreatedEvent = waitResults.Message<SagaCreatedEvent<SoftwareProgrammingSagaData>>();

            Assert.AreEqual(gotTiredEvent.PersonId, expectedCreatedEvent.SagaId);
        }

    }
}