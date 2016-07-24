using System;
using CommonDomain;
using GridDomain.EventSourcing.Sagas;
using GridDomain.Tests.Sagas.StateSagas.SampleSaga;
using NUnit.Framework;

namespace GridDomain.Tests.Sagas.StateSagas.Transitions
{
    [TestFixture]
    public class Given_StateSaga_When_invalid_Transitions
    {
        private readonly Given_State_SoftareProgramming_Saga _given
            = new Given_State_SoftareProgramming_Saga(SoftwareProgrammingSaga.States.Sleeping);

        private class WrongMessage
        {
        }

        private static void When_execute_invalid_transaction(SoftwareProgrammingSaga sagaInstance)
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
            Assert.Throws<UnbindedMessageReceivedException>(
                () => When_execute_invalid_transaction(_given.SagaMachine));
        }

        [Then]
        public void No_events_are_raised_in_data_aggregate()
        {
            var aggregate = (IAggregate)_given.SagaDataAggregate;
            aggregate.ClearUncommittedEvents(); //ignore sagaCreated event
            SwallowException(() => When_execute_invalid_transaction(_given.SagaMachine));
            CollectionAssert.IsEmpty(aggregate.GetUncommittedEvents());
        }

        [Then]
        public void Saga_state_not_changed()
        {
            var stateHashBefore = _given.SagaDataAggregate.MachineState;
            SwallowException(() => When_execute_invalid_transaction(_given.SagaMachine));
            var stateHashAfter = _given.SagaMachine.State.MachineState;

            Assert.AreEqual(stateHashBefore, stateHashAfter);
        }

        [Then]
        public void No_commands_are_proroduced()
        {
            SwallowException(() => When_execute_invalid_transaction(_given.SagaMachine));
            CollectionAssert.IsEmpty(_given.SagaMachine.CommandsToDispatch);
        }
    }
}