using System.Threading.Tasks;
using GridDomain.Aggregates;

namespace GridDomain.Scenarios.Runners {


    public static class AggregateScenarioExtensions
    {

        public static async Task<IAggregateScenarioRun<TAggregate>> Check<TAggregate>(this Task<IAggregateScenarioRun<TAggregate>> scenarioRun) where TAggregate : IAggregate
        {
            var sc = await scenarioRun;
            return sc.Check();
        }

        public static IAggregateScenarioRun<TAggregate> Check<TAggregate>(this IAggregateScenarioRun<TAggregate> scenarioRun) where TAggregate : IAggregate
        {
            LogDebugInfo(scenarioRun);
            EventsExtensions.CompareEvents(scenarioRun.Scenario.ExpectedEvents, scenarioRun.ProducedEvents);
            return scenarioRun;
        }

       
        private static void LogDebugInfo<TAggregate>(IAggregateScenarioRun<TAggregate> scenarioRun) where TAggregate : IAggregate
        {
            var logger = scenarioRun.Log;
            foreach (var cmd in scenarioRun.Scenario.GivenCommands)
                scenarioRun.Log.Information("Command: {@cmd}", cmd);

            logger.Information("Given events:\r\n{@events}",  scenarioRun.Scenario.GivenEvents);
            logger.Information("Produced events:\r\n{@events}",  scenarioRun.ProducedEvents);
            logger.Information("Expected events:\r\n{@events}",  scenarioRun.Scenario.ExpectedEvents);
        }
    }
}