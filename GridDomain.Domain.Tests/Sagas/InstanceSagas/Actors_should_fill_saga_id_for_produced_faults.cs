using System;
using Akka.Actor;
using Akka.TestKit.NUnit3;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.SampleDomain.Events;
using GridDomain.Tests.SampleDomain.ProjectionBuilders;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas
{
    [TestFixture]
    class Actors_should_fill_saga_id_for_produced_faults: TestKit
    {
        [Test]
        public void Aggregate_actor_produce_fault_with_sagaId_from_command()
        {
            var command = new GoSleepCommand(Guid.Empty, Guid.Empty).CloneWithSaga(Guid.NewGuid());

            var transport = new LocalAkkaEventBusTransport(Sys);
            transport.Subscribe<Fault<GoSleepCommand>>(TestActor);

            var actor = Sys.ActorOf(Props.Create(() => 
                new AggregateActor<HomeAggregate>(new HomeAggregateHandler(),
                                                  new TypedMessageActor<ScheduleCommand>(TestActor), 
                                                  new TypedMessageActor<Unschedule>(TestActor),
                                                  transport,
                                                  new SnapshotsPersistencePolicy(TimeSpan.FromSeconds(1),1,5),
                                                  new AggregateFactory()
                                                  )),
                AggregateActorName.New<HomeAggregate>(command.Id).Name );

            actor.Tell(command);
            var fault = ExpectMsg<Fault<GoSleepCommand>>();
            Assert.AreEqual(command.SagaId, fault.SagaId);
        }

        [TestCase("test", Description = "unplanned exception from message processor")]
        [TestCase("10", Description = "planned exception from message processor")]
        public void Message_process_actor_produce_fault_with_sagaId_from_incoming_message(string payload)
        {
            var message = new SampleAggregateChangedEvent(payload,Guid.NewGuid(),DateTime.Now,Guid.NewGuid());

            var transport = new LocalAkkaEventBusTransport(Sys);
            transport.Subscribe<Fault<SampleAggregateChangedEvent>>(TestActor);

            var actor = Sys.ActorOf(Props.Create(() =>
                new MessageHandlingActor<SampleAggregateChangedEvent,OddFaultyMessageHandler>(
                    new OddFaultyMessageHandler(transport), 
                    transport)));

            actor.Tell(message);
            var fault = ExpectMsg<Fault<SampleAggregateChangedEvent>>();
            Assert.AreEqual(message.SagaId, fault.SagaId);
        }


    }
}