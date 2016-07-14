using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Automatonymous;
using CommonDomain;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga;
using GridDomain.Tests.Sagas.SubscriptionRenewSaga.Events;
using KellermanSoftware.CompareNetObjects;
using NUnit.Framework;
using Quartz.Util;

namespace GridDomain.Tests.Sagas.Simplified
{

    class Given_AutomatonymousSaga
    {
        public readonly SubscriptionRenewSagaSimplified SagaMachine = new SubscriptionRenewSagaSimplified();
        public readonly SagaInstance<SubscriptionRenewSagaData> SagaInstance;
        public readonly SagaDataAggregate<SubscriptionRenewSagaData> SagaDataAggregate;

        public Given_AutomatonymousSaga()
        {
            var sagaData = new SubscriptionRenewSagaData(SagaMachine.ChangingSubscription);
            SagaDataAggregate = new SagaDataAggregate<SubscriptionRenewSagaData>(Guid.NewGuid(), sagaData);
            SagaInstance = new SagaInstance<SubscriptionRenewSagaData>(SagaMachine, SagaDataAggregate);
        }
    }

    [TestFixture]
    public class Given_AutomatonymousSaga_When_invalid_Transitions
    {
        private readonly Given_AutomatonymousSaga _given = new Given_AutomatonymousSaga();

        private class WrongMessage
        {
        }

        private static void When_execute_invalid_transaction(SagaInstance<SubscriptionRenewSagaData> sagaInstance)
        {
            sagaInstance.Transit(new WrongMessage());
        }

        private void SwallowException(Action act)
        {
            try
            {
                act();
            }
            catch
            {
                //intentionally left blank
            }
        }
        
        [Then]
        public void Exception_occurs()
        {
            Assert.Throws<UnbindedMessageRecievedException>(() => When_execute_invalid_transaction(_given.SagaInstance));
        }

        [Then]
        public void No_events_are_raised_in_data_aggregate()
        {
            var aggregate = (IAggregate)_given.SagaDataAggregate;
            aggregate.ClearUncommittedEvents(); //ignore sagaCreated event
            SwallowException(() => When_execute_invalid_transaction(_given.SagaInstance));
            CollectionAssert.IsEmpty(aggregate.GetUncommittedEvents());
        }

        [Then]
        public void Saga_state_not_changed()
        {
            var stateHashBefore = _given.SagaDataAggregate.Data.CurrentState.GetHashCode();
            SwallowException(() => When_execute_invalid_transaction(_given.SagaInstance));
            var stateHashAfter = _given.SagaDataAggregate.Data.CurrentState.GetHashCode();

            Assert.AreEqual(stateHashBefore, stateHashAfter);
        }
    }

    [TestFixture]
    internal class AutomatonymousSagas_Transitions_Tests
    {
        private SubscriptionRenewSagaSimplified SagaMachine = new SubscriptionRenewSagaSimplified();


      

        [Test]
        public void When_invalid_transition_Then_state_not_changed()
        {
            var sagaData = new SubscriptionRenewSagaData(SagaMachine.ChangingSubscription);
            var sagaDataAggregate = new SagaDataAggregate<SubscriptionRenewSagaData>(Guid.NewGuid(),sagaData);

            var sagaInstance = new SagaInstance<SubscriptionRenewSagaData>(SagaMachine, sagaDataAggregate);
            sagaInstance.Transit(new SubscriptionExpiredEvent(Guid.NewGuid()));

            Assert.AreEqual(SagaMachine.ChangingSubscription, sagaInstance.Instance.CurrentState);
        }


        [Test]
        public void When_valid_transition_Then_state_is_changed()
        {
            var sagaData = new SubscriptionRenewSagaData(SagaMachine.PayingForSubscription);
            var sagaDataAggregate = new SagaDataAggregate<SubscriptionRenewSagaData>(Guid.NewGuid(), sagaData);

            var sagaInstance = new SagaInstance<SubscriptionRenewSagaData>(SagaMachine, sagaDataAggregate);
            sagaInstance.Transit(new SubscriptionPaidEvent());
            Assert.AreEqual(SagaMachine.SubscriptionSet, sagaInstance.Instance.CurrentState);
        }
    }
}
