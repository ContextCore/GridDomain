using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;
using NUnit.Framework;
using Saga = GridDomain.Tests.Sagas.SubscriptionRenewSaga.SubscriptionRenewSaga;

namespace GridDomain.Tests.Sagas
{
    [TestFixture]
    internal class SubscriptionRenewSagaStateTransitionTests
    {
        private Saga SagaInstance;

        public void Given_new_saga_with_state(Saga.States states)
        {
            var sagaState = new SubscriptionRenewSagaState(Guid.NewGuid(), states);
            SagaInstance = new Saga(sagaState);
        }

        private class WrongMessage
        {
        }


        [Test]
        public void When_invalid_transition_Then_state_not_changed()
        {
            Given_new_saga_with_state(Saga.States.SubscriptionChanging);
            SagaInstance.Transit(new SubscriptionExpiredEvent(Guid.NewGuid()));

            Assert.AreEqual(Saga.States.SubscriptionChanging, SagaInstance.DomainState);
        }

        [Test]
        public void When_unknown_transition_Then_exception_occurs()
        {
            Given_new_saga_with_state(Saga.States.SubscriptionChanging);
            Assert.Throws<UnbindedMessageRecievedException>(() => SagaInstance.TransitState(new WrongMessage()));
        }


        [Test]
        public void When_valid_transition_Then_state_is_changed()
        {
            Given_new_saga_with_state(Saga.States.OfferPaying);
            SagaInstance.Handle(new SubscriptionPaidEvent());

            Assert.AreEqual(Saga.States.SubscriptionSet, SagaInstance.DomainState);
        }
    }
}