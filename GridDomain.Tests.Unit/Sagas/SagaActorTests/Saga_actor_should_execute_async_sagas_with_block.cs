using System;
using Akka.Actor;
using Akka.DI.Core;
using Akka.DI.Unity;
using Akka.TestKit;
using Akka.TestKit.TestActors;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.Actors.CommandPipe;
using GridDomain.Node.Actors.CommandPipe.Messages;
using GridDomain.Node.Actors.CommandPipe.Processors;
using GridDomain.Node.Actors.EventSourced;
using GridDomain.Node.Actors.Sagas;
using GridDomain.Node.Actors.Sagas.Messages;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.Common.Configuration;
using GridDomain.Tests.Unit.BalloonDomain.Events;
using Microsoft.Practices.Unity;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Sagas.SagaActorTests
{
    public class Saga_actor_should_execute_async_sagas_with_block : TestKit
    {
        public Saga_actor_should_execute_async_sagas_with_block(ITestOutputHelper output):base("", output)
        {
            var logger = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
            var creator = new AsyncLongRunningSagaFactory(logger);
            var producer = new Saga—reatorsCatalog<TestState>(AsyncLongRunningProcess.Descriptor, creator);
            producer.RegisterAll(creator);

            _localAkkaEventBusTransport = new LocalAkkaEventBusTransport(Sys);
            var blackHole = Sys.ActorOf(BlackHoleActor.Props);

            var messageProcessActor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(new ProcessorListCatalog(), blackHole)));
            _sagaId = Guid.NewGuid();

            var container = new UnityContainer();

            container.RegisterType<SagaStateActor<TestState>>(new InjectionFactory(cnt =>
                                                                                       new SagaStateActor<TestState>(new SagaStateCommandHandler<TestState>(),
                                                                                                                     _localAkkaEventBusTransport,
                                                                                                                     new EachMessageSnapshotsPersistencePolicy(), new AggregateFactory(),
                                                                                                                     messageProcessActor)));
            Sys.AddDependencyResolver(new UnityDependencyResolver(container, Sys));

            var name = AggregateActorName.New<SagaStateAggregate<TestState>>(_sagaId).Name;
            _sagaActor = ActorOfAsTestActorRef(() => new SagaActor<TestState>(producer,
                                                                              _localAkkaEventBusTransport),
                                               name);
        }

        private readonly TestActorRef<SagaActor<TestState>> _sagaActor;
        private readonly Guid _sagaId;
        private readonly LocalAkkaEventBusTransport _localAkkaEventBusTransport;

        [Fact]
        public void Saga_actor_process_one_message_in_time()
        {
            var domainEventA = new BalloonCreated("1", Guid.NewGuid(), DateTime.Now, _sagaId);
            var domainEventB = new BalloonTitleChanged("2", Guid.NewGuid(), DateTime.Now, _sagaId);

            _sagaActor.Tell(MessageMetadataEnvelop.New(domainEventA, MessageMetadata.Empty));
            _sagaActor.Tell(MessageMetadataEnvelop.New(domainEventB, MessageMetadata.Empty));

            //A was received first and should be processed first
            var msg = ExpectMsg<SagaTransited>();
            Assert.Equal(domainEventA.SourceId,((TestState)msg.NewSagaState).ProcessingId);
            //B should not be processed after A is completed
            var msgB = ExpectMsg<SagaTransited>();
            Assert.Equal(domainEventB.SourceId,((TestState)msgB.NewSagaState).ProcessingId);
        }

        [Fact]
        public void Saga_change_state_after_transitions()
        {
            var domainEventA = new BalloonCreated("1", Guid.NewGuid(), DateTime.Now, _sagaId);

            _sagaActor.Ref.Tell(MessageMetadataEnvelop.New(domainEventA));

            var msg = ExpectMsg<SagaTransited>();

            Assert.Equal(domainEventA.SourceId, ((TestState)msg.NewSagaState).ProcessingId);
        }

        [Fact]
        public void Saga_transition_raises_state_events()
        {
            _sagaActor.Ref.Tell(MessageMetadataEnvelop.New(new BalloonCreated("1", Guid.NewGuid(), DateTime.Now, _sagaId),
                                                           MessageMetadata.Empty));

            _localAkkaEventBusTransport.Subscribe(typeof(IMessageMetadataEnvelop<SagaCreated<TestState>>), TestActor);
            _localAkkaEventBusTransport.Subscribe(typeof(IMessageMetadataEnvelop<SagaReceivedMessage<TestState>>), TestActor);

            FishForMessage<IMessageMetadataEnvelop<SagaCreated<TestState>>>(m => true);
            FishForMessage<IMessageMetadataEnvelop<SagaReceivedMessage<TestState>>>(m => true);
        }
    }
}