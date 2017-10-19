using System;
using System.Threading.Tasks;
using GridDomain.ProcessManagers;
using GridDomain.Tests.Common;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain;
using GridDomain.Tests.Unit.ProcessManagers.SoftwareProgrammingDomain.Events;
using Serilog;
using Xunit;
using Xunit.Abstractions;

namespace GridDomain.Tests.Unit.ProcessManagers.Transitions
{
    public class Given_processActor_When_valid_Transitions
    { 
        public Given_processActor_When_valid_Transitions(ITestOutputHelper output)
        {
            _log = new XUnitAutoTestLoggerConfiguration(output).CreateLogger();
        }

        private readonly ILogger _log;

        [Fact]
        public async Task Commands_are_produced()
        {
            var given = new Given_Automatonymous_Process(m => m.Coding);

            var subscriptionExpiredEvent = new GotTiredEvent(Guid.NewGuid());
            var commands = await given.Process.Transit(given.State, subscriptionExpiredEvent);

            Assert.NotEmpty(commands);
        }

        [Fact]
        public async Task Process_state_is_changed_after_transition_by_event_data()
        {
            var given = new Given_Automatonymous_Process(m => m.Coding);

            var subscriptionExpiredEvent = new GotTiredEvent(Guid.NewGuid());
            await given.Process.Transit(given.State, subscriptionExpiredEvent);

            Assert.Equal(subscriptionExpiredEvent.SourceId, given.State.PersonId);
        }

        [Fact]
        public async Task State_in_transition_result_is_changed()
        {
            var given = new Given_Automatonymous_Process(m => m.MakingCoffee);
            await given.Process.Transit(given.State, new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid()));
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), given.State.CurrentStateName);
        }

        [Fact]
        public async Task State_is_changed_on_using_non_generic_transit_method()
        {
            var given = new Given_Automatonymous_Process(m => m.MakingCoffee);
            object msg = new CoffeMadeEvent(Guid.NewGuid(), Guid.NewGuid());
            await given.Process.Transit(given.State, msg);
            Assert.Equal(nameof(SoftwareProgrammingProcess.Coding), given.State.CurrentStateName);
        }
    }
}