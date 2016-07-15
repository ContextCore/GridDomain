using System;
using System.Collections.Generic;
using Automatonymous;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.Simplified.Transitions
{

    class Given_created_event_when_hydrating: HydrationSpecification<SagaDataAggregate<SubscriptionRenewSagaData>>
    {
        private readonly Guid _sagaId;
        private readonly SubscriptionRenewSagaSimplified _machine;
        private readonly SubscriptionRenewSagaData _subscriptionRenewSagaData;

        public Given_created_event_when_hydrating()
        {
            _sagaId = Guid.NewGuid();
            _machine = new SubscriptionRenewSagaSimplified();
            _subscriptionRenewSagaData = new SubscriptionRenewSagaData(_machine.ChangingSubscription);
        }

        protected override IEnumerable<DomainEvent> GivenEvents()
        {
            yield return new SagaCreatedEvent<SubscriptionRenewSagaData>(_subscriptionRenewSagaData, _sagaId);
        }

        [Test]
        public void Then_State_is_taken_from_event()
        {
            Assert.AreEqual(_subscriptionRenewSagaData,Aggregate.Data);
        }

        [Test]
        public void Then_Id_is_taken_from_event()
        {
            Assert.AreEqual(_sagaId, Aggregate.Id);
        }
    }

    class Given_created_and_message_received_and_transitioned_event_when_hydrating : HydrationSpecification<SagaDataAggregate<SubscriptionRenewSagaData>>
    {
        private readonly Guid _sagaId;
        private readonly SubscriptionRenewSagaSimplified _machine;
        private readonly SubscriptionRenewSagaData _subscriptionRenewSagaData;
        private readonly SubscriptionExpiredEvent _message;

        public Given_created_and_message_received_and_transitioned_event_when_hydrating()
        {
            _sagaId = Guid.NewGuid();
            _machine = new SubscriptionRenewSagaSimplified();
            _subscriptionRenewSagaData = new SubscriptionRenewSagaData(_machine.SubscriptionSet);
            _message = new SubscriptionExpiredEvent(Guid.NewGuid());
        }

        protected override IEnumerable<DomainEvent> GivenEvents()
        {
            yield return new SagaCreatedEvent<SubscriptionRenewSagaData>(_subscriptionRenewSagaData, _sagaId);

            yield return new SagaMessageReceivedEvent<SubscriptionRenewSagaData>(_sagaId,
                                                                                 _subscriptionRenewSagaData, 
                                                                                 _machine.SubscriptionExpired,
                                                                                 _message);

            yield return new SagaTransitionEvent<SubscriptionRenewSagaData>(_sagaId,_subscriptionRenewSagaData, _machine.ChangingSubscription);
        }

        [Test]
        public void Then_State_is_taken_from_transition_event()
        {
            Assert.AreEqual(_subscriptionRenewSagaData, Aggregate.Data);
        }

        [Test]
        public void Then_received_messages_contains_message()
        {
            CollectionAssert.Contains(Aggregate.ReceivedMessages, _message);
        }
    }



    class Given_AutomatonymousSaga
    {
        public readonly SubscriptionRenewSagaSimplified SagaMachine = new SubscriptionRenewSagaSimplified();
        public readonly SagaInstance<SubscriptionRenewSagaData> SagaInstance;
        public readonly SagaDataAggregate<SubscriptionRenewSagaData> SagaDataAggregate;

        public Given_AutomatonymousSaga(Func<SubscriptionRenewSagaSimplified, State> initialState)
        {
            var sagaData = new SubscriptionRenewSagaData(initialState(SagaMachine));
            SagaDataAggregate = new SagaDataAggregate<SubscriptionRenewSagaData>(Guid.NewGuid(), sagaData);
            SagaInstance = new SagaInstance<SubscriptionRenewSagaData>(SagaMachine, SagaDataAggregate);
        }
    }
}