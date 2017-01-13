using System;
using Akka.Actor;
using Akka.TestKit.NUnit3;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Unit.SampleDomain.Events;
using GridDomain.Tests.Unit.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.Unit.Sagas.InstanceSagas.SagaActorTests
{
    [TestFixture]
    class Actors_should_fill_saga_id_for_produced_faults: TestKit
    {
        [Test]
        public void Aggregate_actor_produce_fault_with_sagaId_from_command()
        {
            var command = new GoSleepCommand(Guid.Empty, Guid.Empty).CloneWithSaga(Guid.NewGuid());

            var transport = new LocalAkkaEventBusTransport(Sys);
            transport.Subscribe<MessageMetadataEnvelop<Fault<GoSleepCommand>>>(TestActor);

            var actor = Sys.ActorOf(Props.Create(() => 
                new AggregateActor<HomeAggregate>(new HomeAggregateHandler(),
                                                  TestActor,
                                                  transport,
                                                  new SnapshotsPersistencePolicy(TimeSpan.FromSeconds(1),1,5),
                                                  new AggregateFactory()
                                                  )),
                AggregateActorName.New<HomeAggregate>(command.Id).Name );

            actor.Tell(new MessageMetadataEnvelop<ICommand>(command, new MessageMetadata(command.Id)));

            var fault = ExpectMsg<MessageMetadataEnvelop<Fault<GoSleepCommand>>>(TimeSpan.FromDays(1));

            Assert.AreEqual(command.SagaId, fault.Message.SagaId);
        }

        [TestCase("test", Description = "unplanned exception from message processor")]
        [TestCase("10", Description = "planned exception from message processor")]
        public void Message_process_actor_produce_fault_with_sagaId_from_incoming_message(string payload)
        {
            var message = new SampleAggregateChangedEvent(payload,Guid.NewGuid(),DateTime.Now,Guid.NewGuid());

            var transport = new LocalAkkaEventBusTransport(Sys);
            transport.Subscribe<IMessageMetadataEnvelop<Fault<SampleAggregateChangedEvent>>>(TestActor);

            var actor = Sys.ActorOf(Props.Create(() =>
                new MessageHandlingActor<SampleAggregateChangedEvent,OddFaultyMessageHandler>(
                    new OddFaultyMessageHandler(transport), 
                    transport)));

            actor.Tell(message);
            var fault = ExpectMsg< IMessageMetadataEnvelop<Fault<SampleAggregateChangedEvent>>>();
            Assert.AreEqual(message.SagaId, fault.Message.SagaId);
        }


    }
}