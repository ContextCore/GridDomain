using System;
using System.Threading.Tasks;
using GridDomain.ProcessManagers;
using KellermanSoftware.CompareNetObjects;

namespace GridDomain.Scenarios
{
    public static class ProcessScenarioExtensions
    {
        public static async Task<ProcessScenario<TData>> CheckProducedCommands<TData>(this Task<ProcessScenario<TData>> scenarioInProgress, CompareLogic logic = null) where TData : class, IProcessState
        {
            var scenario = await scenarioInProgress;
            return CheckProducedCommands(scenario, logic);
        }

        public static ProcessScenario<TData> CheckProducedCommands<TData>(this ProcessScenario<TData> scenario, CompareLogic logic=null) where TData : class, IProcessState
        {
            EventsExtensions.CompareCommands(scenario.ExpectedCommands, scenario.ProducedCommands, logic);
            return scenario;
        }

        public static async Task<ProcessScenario< TData>> CheckOnlyStateNameChanged<TData>(this Task<ProcessScenario<TData>> scenarioInProgress, string stateName) where TData : class, IProcessState
        {
            var scenario = await scenarioInProgress;
            CheckOnlyStateNameChanged(scenario, stateName);
            return scenario;
        }

        private static void CheckOnlyStateNameChanged<TData>(this ProcessScenario<TData> scenario, string stateName) where TData : class, IProcessState
        {
            if(stateName != scenario.State.CurrentStateName)
                  throw new StateNameMismatchException();
            EventsExtensions.CompareState(scenario.InitialState,
                                          scenario.State,
                                          Compare.Ignore(nameof(IProcessState.CurrentStateName)));
        }

        private class StateNameMismatchException : Exception { }

        public static async Task<ProcessScenario<TData>> CheckStateName<TData>(this Task<ProcessScenario<TData>> scenarioInProgress, string stateName) where TData : class, IProcessState
        {
            var scenario = await scenarioInProgress;
            return CheckStateName(scenario, stateName);
        }

        public static ProcessScenario<TData> CheckStateName<TData>(this ProcessScenario<TData> scenario, string stateName) where TData : class, IProcessState
        {
            if(stateName != scenario.State.CurrentStateName)
                        throw new StateNameMismatchException();
            return scenario;
        }

        public static async Task<ProcessScenario<TData>> CheckProducedState<TData>(
            this Task<ProcessScenario<TData>> scenarioInProgress, TData expectedState, CompareLogic logic = null)where TData : class, IProcessState
        {
            var scenario = await scenarioInProgress;
            return CheckProducedState(scenario, expectedState, logic);
        }

        private static ProcessScenario<TData> CheckProducedState<TData>(this ProcessScenario<TData> scenario,TData expectedState, CompareLogic logic = null) where TData : class, IProcessState
        {
            EventsExtensions.CompareState(expectedState, scenario.State, logic);
            return scenario;
        }
    }
}