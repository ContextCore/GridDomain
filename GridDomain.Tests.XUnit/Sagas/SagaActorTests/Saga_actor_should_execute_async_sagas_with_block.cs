using System;
using Akka.Actor;
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
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.XUnit.Sagas.SagaActorTests
{
    public class Saga_actor_should_execute_async_sagas_with_block : TestKit
    {
        public Saga_actor_should_execute_async_sagas_with_block(ITestOutputHelper output)
        {
            var logger = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
            var producer = new SagaProducer<ISaga<TestState>>(AsyncLongRunningSaga.Descriptor);
            producer.RegisterAll<AsyncLongRunningSagaFactory, TestState>(new AsyncLongRunningSagaFactory(logger));
            var localAkkaEventBusTransport = new LocalAkkaEventBusTransport(Sys);
            localAkkaEventBusTransport.Subscribe(typeof(object), TestActor);
            var blackHole = Sys.ActorOf(BlackHoleActor.Props);
            var messageProcessActor = Sys.ActorOf(Props.Create(() => new HandlersPipeActor(new ProcessorListCatalog(), blackHole)));
            _sagaId = Guid.NewGuid();

            var name = AggregateActorName.New<SagaStateAggregate<TestState>>(_sagaId).Name;
            _sagaActor = ActorOfAsTestActorRef(() => new SagaActor<TestState>(_sagaId,
                                                                              producer,
                                                                              localAkkaEventBusTransport,
                                                                              blackHole),
                                               name);
        }

        private readonly TestActorRef<SagaActor<TestState>> _sagaActor;
        private readonly Guid _sagaId;

        [Fact]
        public void Saga_actor_blocks_until_async_saga_execution_complete()
        {
            var domainEventA = new SampleAggregateCreatedEvent("1", Guid.NewGuid(), DateTime.Now, _sagaId);
            var domainEventB = new SampleAggregateCreatedEvent("2", Guid.NewGuid(), DateTime.Now, _sagaId);

            _sagaActor.Tell(MessageMetadataEnvelop.New(domainEventA, MessageMetadata.Empty));
            _sagaActor.Tell(MessageMetadataEnvelop.New(domainEventB, MessageMetadata.Empty));

            //B should not be processed until A is completed
            FishForMessage<IMessageMetadataEnvelop<SagaMessageReceivedEvent<TestState>>>(m => true);
            Assert.Equal(domainEventA.SourceId, _sagaActor.UnderlyingActor.Saga.State.ProcessingId);

            //B should not be processed after A is completed
            FishForMessage<IMessageMetadataEnvelop<SagaMessageReceivedEvent<TestState>>>(m => true);
            Assert.Equal(domainEventB.SourceId, _sagaActor.UnderlyingActor.Saga.State.ProcessingId);
        }

        [Fact]
        public void Saga_execute_async_saga_transitions()
        {
            var domainEventA = new SampleAggregateCreatedEvent("1", Guid.NewGuid(), DateTime.Now, _sagaId);

            _sagaActor.Ref.Tell(MessageMetadataEnvelop.New(domainEventA, MessageMetadata.Empty));

            FishForMessage<IMessageMetadataEnvelop<SagaMessageReceivedEvent<TestState>>>(m => true, TimeSpan.FromMinutes(10));

            Assert.Equal(domainEventA.SourceId, _sagaActor.UnderlyingActor.Saga.State.ProcessingId);
        }

        [Fact]
        public void Saga_transition_raises_three_state_events()
        {
            var domainEventA = new SampleAggregateCreatedEvent("1", Guid.NewGuid(), DateTime.Now, _sagaId);

            _sagaActor.Ref.Tell(MessageMetadataEnvelop.New(domainEventA, MessageMetadata.Empty));

            FishForMessage<IMessageMetadataEnvelop<SagaCreatedEvent<TestState>>>(m => true);
            FishForMessage<IMessageMetadataEnvelop<SagaMessageReceivedEvent<TestState>>>(m => true);
        }
    }
}