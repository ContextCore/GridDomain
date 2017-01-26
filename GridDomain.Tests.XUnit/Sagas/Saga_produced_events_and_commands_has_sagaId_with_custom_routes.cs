using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.AkkaMessaging.Waiting;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Events;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace GridDomain.Tests.XUnit.Sagas
{

    public class Saga_produced_events_and_commands_has_sagaId_with_custom_routes : NodeCommandsTest,
                                                                                   IClassFixture<CustomRoutesSampleDomainFixture>
    {
        public Saga_produced_events_and_commands_has_sagaId_with_custom_routes(CustomRoutesSampleDomainFixture fixture, ITestOutputHelper helper) 
            : base(helper, fixture)
        {

        }

        [Fact]
        public void When_dispatch_command_than_command_should_have_right_sagaId()
        { 
            var domainEvent = new GotTiredEvent(Guid.NewGuid());

            NodeTestFixture.GridNode.Pipe.SagaProcessor.Tell(new Initialize(TestActor));
            NodeTestFixture.GridNode.Pipe.SagaProcessor.Tell(new MessageMetadataEnvelop<DomainEvent[]>(new[] { domainEvent },
                                                                                       MessageMetadata.Empty));

            var sagaCompleteMsg = FishForMessage<IMessageMetadataEnvelop<ICommand>>(m => true);
            var command = sagaCompleteMsg.Message;

            Assert.Equal(domainEvent.SagaId, command.SagaId);
            Assert.IsAssignableFrom<MakeCoffeCommand>(command);
        }

        [Fact]
        public async Task When_raise_saga_than_saga_created_event_should_have_right_sagaId()
        {
            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid());

            var waitResults = await NodeTestFixture.GridNode.NewDebugWaiter()
                                                            .Expect<SagaCreatedEvent<SoftwareProgrammingSagaData>>()
                                                            .Create()
                                                            .SendToSaga(gotTiredEvent);

            var expectedCreatedEvent = waitResults.Message<SagaCreatedEvent<SoftwareProgrammingSagaData>>();

            Assert.Equal(gotTiredEvent.PersonId, expectedCreatedEvent.SagaId);
        }

    }
}