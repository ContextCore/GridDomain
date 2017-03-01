using System;
using Akka.Actor;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using GridDomain.Tests.XUnit.SampleDomain.ProjectionBuilders;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.SagaActorTests
{
    public class Actors_should_fill_saga_id_for_produced_faults : TestKit
    {
        [Theory]
        [InlineData("test")] //, Description = "unplanned exception from message processor")]
        [InlineData("10")] //, Description = "planned exception from message processor")]
        public void Message_process_actor_produce_fault_with_sagaId_from_incoming_message(string payload)
        {
            var message = new SampleAggregateChangedEvent(payload, Guid.NewGuid(), DateTime.Now, Guid.NewGuid());

            var transport = new LocalAkkaEventBusTransport(Sys);
            transport.Subscribe<IMessageMetadataEnvelop>(TestActor);

            var actor =
                Sys.ActorOf(
                            Props.Create(
                                         () =>
                                             new MessageHandlingActor<SampleAggregateChangedEvent, OddFaultyMessageHandler>(
                                                                                                                            new OddFaultyMessageHandler(transport),
                                                                                                                            transport)));

            actor.Tell(new MessageMetadataEnvelop<DomainEvent>(message, MessageMetadata.Empty));

            var fault = FishForMessage<IMessageMetadataEnvelop<IFault>>(m => true);

            Assert.Equal(message.SagaId, fault.Message.SagaId);
            Assert.IsAssignableFrom<Fault<SampleAggregateChangedEvent>>(fault.Message);
        }

        [Fact]
        public void Aggregate_actor_produce_fault_with_sagaId_from_command()
        {
            var command = new GoSleepCommand(Guid.Empty, Guid.Empty).CloneWithSaga(Guid.NewGuid());

            var transport = new LocalAkkaEventBusTransport(Sys);
            transport.Subscribe<MessageMetadataEnvelop<Fault<GoSleepCommand>>>(TestActor);

            var actor =
                Sys.ActorOf(
                            Props.Create(
                                         () =>
                                             new AggregateActor<HomeAggregate>(new HomeAggregateHandler(),
                                                                               TestActor,
                                                                               transport,
                                                                               new SnapshotsPersistencePolicy(1, 5, null, null),
                                                                               new AggregateFactory(),
                                                                               TestActor)),
                            AggregateActorName.New<HomeAggregate>(command.Id).Name);

            actor.Tell(new MessageMetadataEnvelop<ICommand>(command, new MessageMetadata(command.Id)));

            var fault = FishForMessage<IMessageMetadataEnvelop<IFault>>(m => true);

            Assert.Equal(command.SagaId, fault.Message.SagaId);
            Assert.IsAssignableFrom<Fault<GoSleepCommand>>(fault.Message);
        }
    }
}