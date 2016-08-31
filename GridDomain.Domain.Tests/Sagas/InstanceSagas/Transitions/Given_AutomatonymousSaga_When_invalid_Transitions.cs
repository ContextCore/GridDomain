using System;
using CommonDomain;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.InstanceSagas.Transitions
{
    [TestFixture]
    public class Given_AutomatonymousSaga_When_invalid_Transitions
    {
        private readonly Given_AutomatonymousSaga _given = new Given_AutomatonymousSaga(m => m.Sleeping);

        private class WrongMessage
        {
        }

        private static void When_execute_invalid_transaction(ISagaInstance sagaInstance)
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
            Assert.Throws<UnbindedMessageReceivedException>(() => When_execute_invalid_transaction(_given.SagaInstance));
        }

        [Then]
        public void Null_message_Exception_occurs()
        {
            Assert.Throws<NullMessageTransitException>(() => _given.SagaInstance.Transit(null));
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
            var stateHashBefore = _given.SagaDataAggregate.Data.CurrentStateName.GetHashCode();
            SwallowException(() => When_execute_invalid_transaction(_given.SagaInstance));
            var stateHashAfter = _given.SagaDataAggregate.Data.CurrentStateName.GetHashCode();

            Assert.AreEqual(stateHashBefore, stateHashAfter);
        }

        [Then]
        public void No_commands_are_proroduced()
        {
            SwallowException(() => When_execute_invalid_transaction(_given.SagaInstance));
            CollectionAssert.IsEmpty(_given.SagaInstance.CommandsToDispatch);
        }
    }
}