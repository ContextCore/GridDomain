using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Automatonymous;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.Simplified
{
    [TestFixture]
    internal class SimplifiedSubscriptionRenewSagaStateTransitionTests
    {
        private SubscriptionRenewSagaSimplified SagaMachine =
            new SubscriptionRenewSagaSimplified();

        private SagaInstance<SubscriptionRenewSagaData> _sagaInstance;

        private class WrongMessage
        {
        }


        [Test]
        public void When_invalid_transition_Then_state_not_changed()
        {
            var sagaData = new SubscriptionRenewSagaData(SagaMachine.ChangingSubscription);
            var sagaDataAggregate = new SagaDataAggregate<SubscriptionRenewSagaData>(Guid.NewGuid(),sagaData);

            _sagaInstance = new SagaInstance<SubscriptionRenewSagaData>(SagaMachine, sagaDataAggregate);
            _sagaInstance.Transit(new SubscriptionExpiredEvent(Guid.NewGuid()));
            Assert.AreEqual(SagaMachine.ChangingSubscription, _sagaInstance.Instance.CurrentState);
        }

        [Test]
        public void When_unknown_event_Then_exception_occurs()
        {
            var sagaData = new SubscriptionRenewSagaData(SagaMachine.ChangingSubscription);
            var sagaDataAggregate = new SagaDataAggregate<SubscriptionRenewSagaData>(Guid.NewGuid(), sagaData);
            _sagaInstance = new SagaInstance<SubscriptionRenewSagaData>(SagaMachine, sagaDataAggregate);
            Assert.Throws<UnbindedMessageRecievedException>(() => _sagaInstance.Transit(new WrongMessage()));
        }


        [Test]
        public void When_valid_transition_Then_state_is_changed()
        {
            var sagaData = new SubscriptionRenewSagaData(SagaMachine.PayingForSubscription);
            var sagaDataAggregate = new SagaDataAggregate<SubscriptionRenewSagaData>(Guid.NewGuid(), sagaData);

            _sagaInstance = new SagaInstance<SubscriptionRenewSagaData>(SagaMachine, sagaDataAggregate);
            _sagaInstance.Transit(new SubscriptionPaidEvent());
            Assert.AreEqual(SagaMachine.SubscriptionSet, _sagaInstance.Instance.CurrentState);
        }
    }
}
