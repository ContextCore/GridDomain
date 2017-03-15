using System;
using System.Threading.Tasks;
using CommonDomain;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.XUnit.CommandsExecution;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using Xunit;
using Xunit.Abstractions;
using ISaga = GridDomain.EventSourcing.Sagas.ISaga;

namespace GridDomain.Tests.XUnit.Sagas.Transitions
{
    public class Given_AutomatonymousSaga_When_invalid_Transitions
    {
        public Given_AutomatonymousSaga_When_invalid_Transitions(ITestOutputHelper output)
        {
            _given = new Given_AutomatonymousSaga(m => m.Sleeping,
                                                  new XUnitAutoTestLoggerConfiguration(output).CreateLogger());
        }

        private readonly Given_AutomatonymousSaga _given;

        private class WrongMessage {}

        private static async Task<StatePreview<SoftwareProgrammingSagaData>> When_execute_invalid_transaction(ISaga<SoftwareProgrammingSaga,SoftwareProgrammingSagaData> saga)
        {
            return await saga.CreateNextState(new WrongMessage());
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
            Assert.ThrowsAsync<UnbindedMessageReceivedException>(
                                                                 async () => await When_execute_invalid_transaction(_given.SagaInstance));
        }

        [Fact]
        public async Task No_commands_are_produced()
        {
            StatePreview<SoftwareProgrammingSagaData> newState = null;
            await SwallowException(async () => newState = await When_execute_invalid_transaction(_given.SagaInstance));
            Assert.Empty(newState?.ProducedCommands);
        }

        [Fact]
        public async Task No_events_are_raised_in_data_aggregate()
        {
            var aggregate = (IAggregate) _given.SagaDataAggregate;
            aggregate.ClearUncommittedEvents(); //ignore sagaCreated event
            await SwallowException(() => When_execute_invalid_transaction(_given.SagaInstance));
            Assert.Empty(aggregate.GetUncommittedEvents());
        }

        [Fact]
        public async Task Null_message_Exception_occurs()
        {
            await _given.SagaInstance.CreateNextState((object) null).ShouldThrow<UnbindedMessageReceivedException>();
        }

        [Fact]
        public async Task Saga_state_not_changed()
        {
            var stateHashBefore = _given.SagaDataAggregate.Data.CurrentStateName.GetHashCode();
            await SwallowException(() => When_execute_invalid_transaction(_given.SagaInstance));
            var stateHashAfter = _given.SagaDataAggregate.Data.CurrentStateName.GetHashCode();

            Assert.Equal(stateHashBefore, stateHashAfter);
        }
    }
}