using System;
using System.Threading.Tasks;
using CommonDomain;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using GridDomain.Tests.Framework;
using GridDomain.Tests.XUnit.CommandsExecution;
using GridDomain.Tests.XUnit.Sagas.SoftwareProgrammingDomain;
using Xunit;
using Xunit.Abstractions;

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
        public async Task Exception_occurs()
        {
            await Assert.ThrowsAsync<UnbindedMessageReceivedException>(() => _given.SagaInstance
                                                                                   .PreviewTransit(new WrongMessage()));
        }

        [Fact]
        public async Task No_commands_are_produced()
        {
            TransitionResult<SoftwareProgrammingSagaState> newState = null;
            await SwallowException(async () => newState = await _given.SagaInstance.PreviewTransit(new WrongMessage()));
            Assert.Null(newState?.ProducedCommands);
        }

        [Fact]
        public async Task Null_message_Exception_occurs()
        {
            await Assert.ThrowsAsync<UnbindedMessageReceivedException>(() => _given.SagaInstance
                                                                             .PreviewTransit((object) null));
        }

        [Fact]
        public async Task Saga_state_not_changed()
        {
            var stateHashBefore = _given.SagaInstance.State.CurrentStateName;

            await SwallowException(() => _given.SagaInstance.PreviewTransit(new WrongMessage()));

            var stateHashAfter = _given.SagaInstance.State.CurrentStateName;

            Assert.Equal(stateHashBefore, stateHashAfter);
        }
    }
}