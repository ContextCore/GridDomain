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
using GridDomain.Node.Actors.CommandPipe.ProcessorCatalogs;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Microsoft.Practices.Unity;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas.SagaActorTests
{
    public class Saga_actor_should_execute_async_sagas_with_block : TestKit
    {
        public Saga_actor_should_execute_async_sagas_with_block(ITestOutputHelper output)
        {
            var logger = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
            var producer = new SagaProducer<TestState>(AsyncLongRunningSaga.Descriptor);
            producer.RegisterAll(new AsyncLongRunningSagaFactory(logger));
            var localAkkaEventBusTransport = new LocalAkkaEventBusTransport(Sys);
            localAkkaEventBusTransport.Subscribe(typeof(object), TestActor);
            var blackHole = Sys.ActorOf(BlackHoleActor.Props);
            var messageProcessActor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(new ProcessorListCatalog(), blackHole)));
            _sagaId = Guid.NewGuid();

            var container = new UnityContainer();
            
            container.RegisterType<AggregateActor<SagaStateAggregate<TestState>>>(new InjectionFactory( cnt =>
             new AggregateActor<SagaStateAggregate<TestState>>(new SagaStateCommandHandler<TestState>(),
                blackHole,
                localAkkaEventBusTransport,
                new EachMessageSnapshotsPersistencePolicy(), new AggregateFactory(), messageProcessActor)));

            Sys.AddDependencyResolver(new UnityDependencyResolver(container, Sys));

            var name = AggregateActorName.New<SagaStateAggregate<TestState>>(_sagaId).Name;
            _sagaActor = ActorOfAsTestActorRef(() => new SagaActor<TestState>(producer,
                                                                              localAkkaEventBusTransport),
                                               name);

            _sagaActor.Ask<NotifyOnSagaTransitedAck>(new NotifyOnSagaTransited(TestActor)).Wait();

        }

        private readonly TestActorRef<SagaActor<TestState>> _sagaActor;
        private readonly Guid _sagaId;

        [Fact]
        public void Saga_actor_process_one_message_in_time()
        {
            var domainEventA = new SampleAggregateCreatedEvent("1", Guid.NewGuid(), DateTime.Now, _sagaId);
            var domainEventB = new SampleAggregateCreatedEvent("2", Guid.NewGuid(), DateTime.Now, _sagaId);

            _sagaActor.Tell(MessageMetadataEnvelop.New(domainEventA, MessageMetadata.Empty));
            _sagaActor.Tell(MessageMetadataEnvelop.New(domainEventB, MessageMetadata.Empty));

            //A was received first and should be processed first
            FishForMessage<IMessageMetadataEnvelop<SagaMessageReceivedEvent<TestState>>>(m => true);
            FishForMessage<SagaTransited>(m => true);
            Assert.Equal(domainEventA.SourceId, _sagaActor.UnderlyingActor.Saga.State.ProcessingId);

            //B should not be processed after A is completed
            FishForMessage<IMessageMetadataEnvelop<SagaMessageReceivedEvent<TestState>>>(m => true);
            FishForMessage<SagaTransited>(m => true);
            Assert.Equal(domainEventB.SourceId, _sagaActor.UnderlyingActor.Saga.State.ProcessingId);
        }

        [Fact]
        public void Saga_change_state_after_transitions()
        {
            var domainEventA = new SampleAggregateCreatedEvent("1", Guid.NewGuid(), DateTime.Now, _sagaId);

            _sagaActor.Ref.Tell(MessageMetadataEnvelop.New(domainEventA, MessageMetadata.Empty));

            FishForMessage<SagaTransited>(m => true);

            Assert.Equal(domainEventA.SourceId, _sagaActor.UnderlyingActor.Saga.State.ProcessingId);
        }

        [Fact]
        public void Saga_transition_raises_state_events()
        {
            var domainEventA = new SampleAggregateCreatedEvent("1", Guid.NewGuid(), DateTime.Now, _sagaId);

            _sagaActor.Ref.Tell(MessageMetadataEnvelop.New(domainEventA, MessageMetadata.Empty));

            FishForMessage<IMessageMetadataEnvelop<SagaCreatedEvent<TestState>>>(m => true);
            FishForMessage<IMessageMetadataEnvelop<SagaMessageReceivedEvent<TestState>>>(m => true);
        }
    }
}