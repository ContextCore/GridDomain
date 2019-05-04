using System.Threading.Tasks;
using GridDomain.Aggregates;
using Microsoft.Extensions.Logging;

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
                scenarioRun.Log.LogInformation("Command: {@cmd}", cmd);

            logger.LogInformation("Given events:\r\n{@events}",  scenarioRun.Scenario.GivenEvents);
            logger.LogInformation("Produced events:\r\n{@events}",  scenarioRun.ProducedEvents);
            logger.LogInformation("Expected events:\r\n{@events}",  scenarioRun.Scenario.ExpectedEvents);
        }
    }
}