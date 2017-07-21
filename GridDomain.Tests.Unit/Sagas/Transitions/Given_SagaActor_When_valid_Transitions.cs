using System;
using System.Threading.Tasks;
using GridDomain.Processes;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.Sagas.SoftwareProgrammingDomain.Events;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.Sagas.Transitions
{
    public class Given_SagaActor_When_valid_Transitions
    { 
        public Given_SagaActor_When_valid_Transitions(ITestOutputHelper output)
        {
            _log = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
        }

        private readonly ILogger _log;

        [Fact]
        public async Task Commands_are_produced()
        {
            var given = new Given_AutomatonymousSaga(m => m.Coding, _log);

            var subscriptionExpiredEvent = new GotTiredEvent(Guid.NewGuid());
            var newState = await ((IProcessManager<SoftwareProgrammingState>) given.ProcessManagerInstance).Transit(subscriptionExpiredEvent);

            Assert.NotEmpty(newState.ProducedCommands);
        }

        [Fact]
        public async Task SagaData_is_changed_after_transition_by_event_data()
        {
            var given = new Given_AutomatonymousSaga(m => m.Coding, _log);

            var subscriptionExpiredEvent = new GotTiredEvent(Guid.NewGuid());
            var newState = await ((IProcessManager<SoftwareProgrammingState>) given.ProcessManagerInstance).Transit(subscriptionExpiredEvent);

            Assert.Equal(subscriptionExpiredEvent.SourceId, newState.State.PersonId);
        }

        [Fact]
        public async Task State_in_transition_result_is_changed()
        {
            var given = new Given_AutomatonymousSaga(m => m.MakingCoffee, _log);
            var newState = await ((IProcessManager<SoftwareProgrammingState>) given.ProcessManagerInstance).Transit(new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid()));
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), newState.State.CurrentStateName);
        }

        [Fact]
        public async Task Saga_state_not_changed()
        {
            var given = new Given_AutomatonymousSaga(m => m.MakingCoffee, _log);

            var stateBefore = given.ProcessManagerInstance.State.CurrentStateName;

            await given.ProcessManagerInstance.Transit(new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid()));

            var stateAfter = given.ProcessManagerInstance.State.CurrentStateName;

            Assert.Equal(stateBefore, stateAfter);
        }

        [Fact]
        public async Task State_is_changed_on_using_non_generic_transit_method()
        {
            var given = new Given_AutomatonymousSaga(m => m.MakingCoffee, _log);
            object msg = new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid());
            var newState =  await given.ProcessManagerInstance.Transit((dynamic)msg);
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), newState.State.CurrentStateName);
        }

        [Fact]
        public async Task When_apply_known_but_not_mapped_event_in_state()
        {
            var given = new Given_AutomatonymousSaga(m => m.Sleeping, _log);
            var gotTiredEvent = new GotTiredEvent(Guid.NewGuid());
            await XUnitAssertExtensions.ShouldThrow<ProcessTransitionException>(given.ProcessManagerInstance.Transit(gotTiredEvent));
        }
    }
}