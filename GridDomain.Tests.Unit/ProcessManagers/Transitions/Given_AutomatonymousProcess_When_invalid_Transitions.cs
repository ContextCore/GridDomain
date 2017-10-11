using System;
using System.Threading.Tasks;
using GridDomain.ProcessManagers;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers.Transitions
{
    public class Given_AutomatonymousProcess_When_invalid_Transitions
    {
        public Given_AutomatonymousProcess_When_invalid_Transitions(ITestOutputHelper output)
        {
            _given = new Given_Automatonymous_Process(m => m.Sleeping);
        }

        private readonly Given_Automatonymous_Process _given;

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
            await Assert.ThrowsAsync<UnbindedMessageReceivedException>(() => _given.Process
                                                                                   .Transit(_given.State, new WrongMessage()));
        }

        [Fact]
        public async Task No_commands_are_produced()
        {
            ProcessResult<SoftwareProgrammingState> newState = null;
            await SwallowException(async () => newState = await _given.Process.Transit(_given.State, new WrongMessage()));
            Assert.Null(newState?.ProducedCommands);
        }

        [Fact]
        public async Task Null_message_Exception_occurs()
        {
            await Assert.ThrowsAsync<UnbindedMessageReceivedException>(() => _given.Process
                                                                             .Transit(_given.State, null));
        }

        [Fact]
        public async Task Process_state_not_changed()
        {
            var stateHashBefore = _given.State.CurrentStateName;

            await SwallowException(() => _given.Process.Transit(_given.State, new WrongMessage()));

            var stateHashAfter = _given.State.CurrentStateName;

            Assert.Equal(stateHashBefore, stateHashAfter);
        }
    }
}