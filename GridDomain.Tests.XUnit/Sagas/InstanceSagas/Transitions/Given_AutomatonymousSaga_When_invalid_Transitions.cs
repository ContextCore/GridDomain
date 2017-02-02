using System;
using System.Threading.Tasks;
using CommonDomain;
using GridDomain.EventSourcing.Sagas;
using Xunit;

namespace GridDomain.Tests.XUnit.Sagas.InstanceSagas.Transitions
{
   
    public class Given_AutomatonymousSaga_When_invalid_Transitions
    {
        private readonly Given_AutomatonymousSaga _given = new Given_AutomatonymousSaga(m => m.Sleeping);

        private class WrongMessage
        {
        }

        private static async Task When_execute_invalid_transaction(ISagaInstance sagaInstance)
        {
            await sagaInstance.Transit(new WrongMessage());
        }

        private async Task SwallowException(Func<Task> act)
        {
            try
            {
               await act();
            }
            catch
            {
                //intentionally left blank
            }
        }
        
      [Fact]
        public void Exception_occurs()
        {
            Assert.ThrowsAsync<UnbindedMessageReceivedException>(async () => await When_execute_invalid_transaction(_given.SagaInstance));
        }

      [Fact]
        public void Null_message_Exception_occurs()
        {
            Assert.ThrowsAsync<UnbindedMessageReceivedException>(async () => await  _given.SagaInstance.Transit((object)null));
        }

      [Fact]
        public async Task No_events_are_raised_in_data_aggregate()
        {
            var aggregate = (IAggregate)_given.SagaDataAggregate;
            aggregate.ClearUncommittedEvents(); //ignore sagaCreated event
            await SwallowException(() => When_execute_invalid_transaction(_given.SagaInstance));
            Assert.Empty(aggregate.GetUncommittedEvents());
        }

      [Fact]
        public async Task Saga_state_not_changed()
        {
            var stateHashBefore = _given.SagaDataAggregate.Data.CurrentStateName.GetHashCode();
            await SwallowException(() => When_execute_invalid_transaction(_given.SagaInstance));
            var stateHashAfter = _given.SagaDataAggregate.Data.CurrentStateName.GetHashCode();

            Assert.Equal(stateHashBefore, stateHashAfter);
        }

      [Fact]
        public async Task No_commands_are_proroduced()
        {
            await SwallowException(() => When_execute_invalid_transaction(_given.SagaInstance));
            Assert.Empty(_given.SagaInstance.CommandsToDispatch);
        }
    }
}