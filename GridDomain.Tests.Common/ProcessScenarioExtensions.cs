using System.Threading.Tasks;
using GridDomain.ProcessManagers;
using KellermanSoftware.CompareNetObjects;
using Xunit;

namespace GridDomain.Tests.Common
{
    public static class ProcessScenarioExtensions
    {
        public static async Task<ProcessScenario<TProcess, TData>> CheckProducedCommands<TProcess, TData>(this Task<ProcessScenario<TProcess, TData>> scenarioInProgress, CompareLogic logic = null) where TProcess : Process<TData> where TData : class, IProcessState
        {
            var scenario = await scenarioInProgress;
            EventsExtensions.CompareCommands(scenario.ExpectedCommands, scenario.ProducedCommands,logic);
            return scenario;
        }

        public static async Task<ProcessScenario<TProcess, TData>> CheckOnlyStateNameChanged<TProcess, TData>(this Task<ProcessScenario<TProcess, TData>> scenarioInProgress, string stateName) where TProcess : Process<TData> where TData : class, IProcessState
        {
            var scenario = await scenarioInProgress;
            Assert.Equal(stateName, scenario.ProcessManager.State.CurrentStateName);
            EventsExtensions.CompareState(scenario.InitialState,
                                          scenario.ProcessManager.State,
                                          Compare.Ignore(nameof(IProcessState.CurrentStateName)));
            return scenario;
        }
        public static async Task<ProcessScenario<TProcess, TData>> CheckStateName<TProcess, TData>(this Task<ProcessScenario<TProcess, TData>> scenarioInProgress, string stateName) where TProcess : Process<TData> where TData : class, IProcessState
        {
            var scenario = await scenarioInProgress;
            Assert.Equal(stateName, scenario.ProcessManager.State.CurrentStateName);
            return scenario;
        }
 

        public static async Task<ProcessScenario<TProcess, TData>> CheckProducedState<TProcess, TData>(
            this Task<ProcessScenario<TProcess, TData>> scenarioInProgress, TData expectedState, CompareLogic logic = null) where TProcess : Process<TData> where TData : class, IProcessState
        {
            var scenario = await scenarioInProgress;
            EventsExtensions.CompareState(expectedState, scenario.ProcessManager.State, logic);
            return scenario;
        }
    }
}