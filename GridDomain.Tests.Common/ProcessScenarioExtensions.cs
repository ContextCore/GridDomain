using System.Threading.Tasks;
using GridDomain.ProcessManagers;
using KellermanSoftware.CompareNetObjects;
using Xunit;

namespace GridDomain.Tests.Common
{
    public static class ProcessScenarioExtensions
    {
        public static async Task<ProcessScenario<TData>> CheckProducedCommands<TData>(this Task<ProcessScenario<TData>> scenarioInProgress, CompareLogic logic = null) where TData : class, IProcessState
        {
            var scenario = await scenarioInProgress;
            EventsExtensions.CompareCommands(scenario.ExpectedCommands, scenario.ProducedCommands,logic);
            return scenario;
        }

        public static async Task<ProcessScenario< TData>> CheckOnlyStateNameChanged<TData>(this Task<ProcessScenario<TData>> scenarioInProgress, string stateName) where TData : class, IProcessState
        {
            var scenario = await scenarioInProgress;
            Assert.Equal(stateName, scenario.State.CurrentStateName);
            EventsExtensions.CompareState(scenario.InitialState,
                                          scenario.State,
                                          Compare.Ignore(nameof(IProcessState.CurrentStateName)));
            return scenario;
        }
        public static async Task<ProcessScenario<TData>> CheckStateName<TData>(this Task<ProcessScenario<TData>> scenarioInProgress, string stateName) where TData : class, IProcessState
        {
            var scenario = await scenarioInProgress;
            Assert.Equal(stateName, scenario.State.CurrentStateName);
            return scenario;
        }
 

        public static async Task<ProcessScenario<TData>> CheckProducedState<TData>(
            this Task<ProcessScenario<TData>> scenarioInProgress, TData expectedState, CompareLogic logic = null)where TData : class, IProcessState
        {
            var scenario = await scenarioInProgress;
            EventsExtensions.CompareState(expectedState, scenario.State, logic);
            return scenario;
        }
    }
}