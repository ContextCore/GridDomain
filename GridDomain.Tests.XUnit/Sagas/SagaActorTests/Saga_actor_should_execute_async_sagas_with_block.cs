using System;
using Akka.Actor;
using Akka.TestKit;
using Akka.TestKit.Xunit2;
using GridDomain.Common;
using GridDomain.CQRS.Messaging.Akka;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Node.Actors;
using GridDomain.Node.AkkaMessaging;
using GridDomain.Tests.XUnit.SampleDomain.Events;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.SagaActorTests
{
   
    public class Saga_actor_should_execute_async_sagas_with_block : TestKit
    {
        private TestActorRef<SagaActor<ISagaInstance<AsyncLongRunningSaga, TestState>, SagaStateAggregate<TestState>>> _actor;
        private Guid _sagaId;
        private LocalAkkaEventBusTransport _localAkkaEventBusTransport;


        public Saga_actor_should_execute_async_sagas_with_block()
        {
            var producer = new SagaProducer<ISagaInstance<AsyncLongRunningSaga, TestState>>(AsyncLongRunningSaga.Descriptor);
            producer.RegisterAll<AsyncLongRunningSagaFactory, TestState>(new AsyncLongRunningSagaFactory());
            _localAkkaEventBusTransport = new LocalAkkaEventBusTransport(Sys);
            _localAkkaEventBusTransport.Subscribe(typeof(object),TestActor);

            var props =
                Props.Create(
                    () => new SagaActor<ISagaInstance<AsyncLongRunningSaga, TestState>, SagaStateAggregate<TestState>>(
                                                                        producer,
                                                                        _localAkkaEventBusTransport,
                                                                        new EachMessageSnapshotsPersistencePolicy(),
                                                                        new AggregateFactory()));

            _sagaId = Guid.NewGuid();
            var name = AggregateActorName.New<SagaStateAggregate<TestState>>(_sagaId).Name;

            _actor = ActorOfAsTestActorRef<SagaActor<ISagaInstance<AsyncLongRunningSaga, TestState>, SagaStateAggregate<TestState>>>(props,name);

        }

        [Fact]
        public void Saga_execute_async_saga_transitions()
        {
            Assert.NotNull(_actor);

            var domainEventA = new SampleAggregateCreatedEvent("1", Guid.NewGuid(), DateTime.Now, _sagaId);

            _actor.Ref.Tell(MessageMetadataEnvelop.New(domainEventA, MessageMetadata.Empty));
            FishForMessage<IMessageMetadataEnvelop<SagaMessageReceivedEvent<TestState>>>(m => true);

            Assert.Equal(domainEventA.SourceId, _actor.UnderlyingActor.Saga.Data.Data.ProcessingId);
        }

        [Fact]
        public void Saga_transition_raises_three_state_events()
        {
            Assert.NotNull(_actor);

            var domainEventA = new SampleAggregateCreatedEvent("1", Guid.NewGuid(), DateTime.Now, _sagaId);

            _actor.Ref.Tell(MessageMetadataEnvelop.New(domainEventA, MessageMetadata.Empty));

            FishForMessage<IMessageMetadataEnvelop<SagaCreatedEvent<TestState>>>(m => true);
            FishForMessage<IMessageMetadataEnvelop<SagaMessageReceivedEvent<TestState>>>(m => true);
        }

        [Fact]
        public void Saga_actor_blocks_until_async_saga_execution_complete()
        {
            Assert.NotNull(_actor);

            var domainEventA = new SampleAggregateCreatedEvent("1",Guid.NewGuid(),DateTime.Now, _sagaId);
            var domainEventB = new SampleAggregateCreatedEvent("2",Guid.NewGuid(),DateTime.Now, _sagaId);

            _actor.Tell(MessageMetadataEnvelop.New(domainEventA, MessageMetadata.Empty));
            _actor.Tell(MessageMetadataEnvelop.New(domainEventB, MessageMetadata.Empty));

            //B should not be processed until A is completed
            FishForMessage<IMessageMetadataEnvelop<SagaMessageReceivedEvent<TestState>>>(m => true);
            Assert.Equal(domainEventA.SourceId, _actor.UnderlyingActor.Saga.Data.Data.ProcessingId);

            //B should not be processed after A is completed
            FishForMessage<IMessageMetadataEnvelop<SagaMessageReceivedEvent<TestState>>>(m => true);
            Assert.Equal(domainEventB.SourceId, _actor.UnderlyingActor.Saga.Data.Data.ProcessingId);
        }

    }
}