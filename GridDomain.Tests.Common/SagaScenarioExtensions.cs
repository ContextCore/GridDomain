using System.Threading.Tasks;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.InstanceSagas;
using KellermanSoftware.CompareNetObjects;
using Xunit;

namespace GridDomain.Tests.Common
{
    public static class SagaScenarioExtensions
    {
        public static async Task<SagaScenario<TSaga, TData>> CheckProducedCommands<TSaga, TData>(this Task<SagaScenario<TSaga, TData>> scenarioInProgress) where TSaga : Process<TData> where TData : class, ISagaState
        {
            var scenario = await scenarioInProgress;
            EventsExtensions.CompareCommands(scenario.ExpectedCommands, scenario.ProducedCommands);
            return scenario;
        }

        public static async Task<SagaScenario<TSaga, TData>> CheckOnlyStateNameChanged<TSaga, TData>(this Task<SagaScenario<TSaga, TData>> scenarioInProgress, string stateName) where TSaga : Process<TData> where TData : class, ISagaState
        {
            var scenario = await scenarioInProgress;
            Assert.Equal(stateName, scenario.Saga.State.CurrentStateName);
            EventsExtensions.CompareStateWithoutName(scenario.InitialState, scenario.Saga.State);
            return scenario;
        }

        public static async Task<SagaScenario<TSaga, TData>> CheckProducedState<TSaga, TData>(
            this Task<SagaScenario<TSaga, TData>> scenarioInProgress, TData expectedState, CompareLogic logic = null) where TSaga : Process<TData> where TData : class, ISagaState
        {
            var scenario = await scenarioInProgress;
            EventsExtensions.CompareState(expectedState, scenario.Saga.State, logic);
            return scenario;
        }
    }
}