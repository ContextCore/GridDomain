using System;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;
using NUnit.Framework;
using Saga = GridDomain.Tests.Sagas.SubscriptionRenewSaga.SubscriptionRenewSaga;
namespace GridDomain.Tests.Sagas
{

    [TestFixture]
    class CreateSagaGraph
    {
        [Test]
        public void GEtGraph()
        {
            var saga = new Saga(new SagaStateAggregate<Saga.States, Saga.Triggers>(Guid.NewGuid(),Saga.States.SubscriptionSet));
            Console.WriteLine(saga.Machine.ToDotGraph());
        }
    }


    [TestFixture]
    class SagaStateTransitionTests
    {
        private SubscriptionRenewSaga.SubscriptionRenewSaga Saga;

        public void Given_new_saga_with_state(SubscriptionRenewSaga.SubscriptionRenewSaga.States states)
        {
            var sagaState = new SagaStateAggregate<SubscriptionRenewSaga.SubscriptionRenewSaga.States,
                SubscriptionRenewSaga.SubscriptionRenewSaga.Triggers>(Guid.NewGuid(),states);
            Saga = new SubscriptionRenewSaga.SubscriptionRenewSaga(sagaState);
        }


        [Test]
        public void When_valid_transition_Then_state_is_changed()
        {
            Given_new_saga_with_state(SubscriptionRenewSaga.SubscriptionRenewSaga.States.OfferPaying);
            Saga.Handle(new SubscriptionPaidEvent());

            Assert.AreEqual(SubscriptionRenewSaga.SubscriptionRenewSaga.States.SubscriptionSet, Saga.State);
        }


        [Test]
        public void When_invalid_transition_Then_state_not_changed()
        {
            Given_new_saga_with_state(SubscriptionRenewSaga.SubscriptionRenewSaga.States.SubscriptionChanging);
            Saga.Handle(new SubscriptionExpiredEvent());

            Assert.AreEqual(SubscriptionRenewSaga.SubscriptionRenewSaga.States.SubscriptionChanging, Saga.State);
        }

        private class WrongMessage { }
        
        [Test]
        public void When_unknown_transition_Then_exception_occurs()
        {
            Given_new_saga_with_state(SubscriptionRenewSaga.SubscriptionRenewSaga.States.SubscriptionChanging);
            Assert.Throws<UnbindedMessageRecievedException>(() => Saga.Transit(new WrongMessage()));
        }
    }
}
