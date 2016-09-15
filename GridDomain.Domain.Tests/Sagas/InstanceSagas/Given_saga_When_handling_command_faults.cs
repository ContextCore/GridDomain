using System;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.TestKit.NUnit;
using GridDomain.Common;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Node.Configuration.Composition;
using GridDomain.Scheduling.Akka.Messages;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Commands;
using GridDomain.Tests.Sagas.SoftwareProgrammingDomain.Events;
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

            var transport = new AkkaEventBusTransport(Sys);
            transport.Subscribe<Fault<GoSleepCommand>>(TestActor);

            var actor = Sys.ActorOf(Props.Create(() => 
            new AggregateActor<HomeAggregate>(new HomeAggregateHandler(),
                                              new AggregateFactory(), 
                                              new TypedMessageActor<ScheduleCommand>(TestActor), 
                                              new TypedMessageActor<Unschedule>(TestActor),
                                              transport)),
            AggregateActorName.New<HomeAggregate>(command.Id).Name);

            actor.Tell(command);
            var fault = ExpectMsg<Fault<GoSleepCommand>>();
            Assert.AreEqual(command.SagaId, fault.SagaId);
        }

        [TestCase("test", "", "unplanned exception from message processor")]
        [TestCase("10", "", "planned exception from message processor")]
        public void Message_process_actor_produce_fault_with_sagaId_from_incoming_message(string payload)
        {
            var message = new SampleAggregateChangedEvent(payload,Guid.NewGuid(),DateTime.Now,Guid.NewGuid());

            var transport = new AkkaEventBusTransport(Sys);
            transport.Subscribe<Fault<GoSleepCommand>>(TestActor);

            var actor = Sys.ActorOf(Props.Create(() =>
            new MessageHandlingActor<SampleAggregateChangedEvent,OddFaultyMessageHandler>(
                                              new OddFaultyMessageHandler(transport), 
                                              transport)));

            actor.Tell(message);
            var fault = ExpectMsg<Fault<SampleAggregateChangedEvent>>();
            Assert.AreEqual(message.SagaId, fault.SagaId);
        }


    }


    [TestFixture]
    class Given_saga_When_handling_command_faults : ProgrammingSoftwareSagaTest
    {
        private SagaDataAggregate<SoftwareProgrammingSagaData> _sagaDataAggregate;
        private CoffeMakeFailedEvent _coffeMaidEvent;
        private SoftwareProgrammingSagaData _sagaData;

        protected override IContainerConfiguration CreateConfiguration()
        {
            return new CustomContainerConfiguration(c => base.CreateConfiguration().Register(c), c => c.RegisterAggregate<HomeAggregate,HomeAggregateHandler>());
        }

        protected override IMessageRouteMap CreateMap()
        {
            return new CustomRouteMap(r => base.CreateMap().Register(r),
                r => r.RegisterAggregate(HomeAggregateHandler.Descriptor));
        }

        [TestFixtureSetUp]
        public void When_publishing_start_message()
        {
            var sagaId = Guid.NewGuid();
            _sagaData = new SoftwareProgrammingSagaData(nameof(SoftwareProgrammingSaga.MakingCoffee))
            {
                PersonId = Guid.NewGuid()
            };

            var sagaDataEvent = new SagaCreatedEvent<SoftwareProgrammingSagaData>(_sagaData, sagaId);

            SaveInJournal<SagaDataAggregate<SoftwareProgrammingSagaData>>(sagaId,sagaDataEvent);

            Thread.Sleep(100);
            _coffeMaidEvent = (CoffeMakeFailedEvent) new CoffeMakeFailedEvent(Guid.NewGuid(), Guid.Empty).CloneWithSaga(sagaId);

            GridNode.Transport.Publish(_coffeMaidEvent);

            //wait for transition after command fault execution
            WaitFor<GoSleepCommand>();

            var fault = (IFault)WaitFor<Fault<GoSleepCommand>>(false).Message;
            WaitFor<SagaTransitionEvent<SoftwareProgrammingSagaData>>();

            _sagaDataAggregate = LoadAggregate<SagaDataAggregate<SoftwareProgrammingSagaData>>(sagaId);
        }


        [Then]
        public void Saga_should_receive_fault_message()
        {
            CollectionAssert.IsNotEmpty(_sagaDataAggregate.ReceivedMessages.OfType<IFault<GoSleepCommand>>());
        }

        [Then]
        public void Saga_state_should_contain_data_from_fault_message()
        {
            var fault = _sagaDataAggregate.ReceivedMessages.OfType<IFault<GoSleepCommand>>().First();
            Assert.AreEqual(_sagaData.PersonId, fault.Message.PersonId);
        }
    }
}