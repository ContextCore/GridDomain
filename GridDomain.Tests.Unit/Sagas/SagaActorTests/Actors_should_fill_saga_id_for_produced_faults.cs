using System;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.Aggregates;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.Processors;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.Hadlers;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using GridDomain.Tests.Unit.BalloonDomain.ProjectionBuilders;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Commands;
using Xunit;

namespace GridDomain.Tests.Unit.Sagas.SagaActorTests
{
    public class Actors_should_fill_saga_id_for_produced_faults : TestKit
    {
        [Theory]
        [InlineData("test")] //, Description = "unplanned exception from message processor")]
        [InlineData("10")] //, Description = "planned exception from message processor")]
        public void Message_process_actor_produce_fault_with_sagaId_from_incoming_message(string payload)
        {
            var message = new BalloonTitleChanged(payload, Guid.NewGuid(), DateTime.Now, Guid.NewGuid());

            var transport = new LocalAkkaEventBusTransport(Sys);
            transport.Subscribe<IMessageMetadataEnvelop>(TestActor);

            var actor =
                Sys.ActorOf(
                            Props.Create(
                                         () =>
                                             new MessageProcessActor<BalloonTitleChanged, BalloonTitleChangedOddFaultyMessageHandler>(
                                                                                                                   new BalloonTitleChangedOddFaultyMessageHandler(transport),
                                                                                                                   transport)));

            actor.Tell(new MessageMetadataEnvelop<DomainEvent>(message, MessageMetadata.Empty));

            var fault = FishForMessage<IMessageMetadataEnvelop<IFault>>(m => true);

            Assert.Equal(message.SagaId, fault.Message.SagaId);
            Assert.IsAssignableFrom<Fault<BalloonTitleChanged>>(fault.Message);
        }

        [Fact]
        public void Aggregate_actor_produce_fault_with_sagaId_from_command()
        {
            var command = new GoSleepCommand(Guid.Empty, Guid.Empty).CloneWithSaga(Guid.NewGuid());

            var transport = new LocalAkkaEventBusTransport(Sys);
            transport.Subscribe<MessageMetadataEnvelop<Fault<GoSleepCommand>>>(TestActor);
            var handlersActor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(new ProcessorListCatalog(), TestActor)));

            var actor = Sys.ActorOf(Props.Create(() => new AggregateActor<HomeAggregate>(new HomeAggregateHandler(),
                                                                                         transport,
                                                                                         new SnapshotsPersistencePolicy(1, 5, null, null),
                                                                                         new AggregateFactory(),
                                                                                         handlersActor)),
                            AggregateActorName.New<HomeAggregate>(command.Id).Name);

            actor.Tell(new MessageMetadataEnvelop<ICommand>(command, new MessageMetadata(command.Id)));

            var fault = FishForMessage<MessageMetadataEnvelop<Fault<GoSleepCommand>>>(m => true,TimeSpan.FromMinutes(100));

            Assert.Equal(command.SagaId, fault.Message.SagaId);
        }
    }
}