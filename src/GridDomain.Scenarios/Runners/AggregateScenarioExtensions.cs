using System.Threading.Tasks;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;
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
            foreach (var planResult in scenarioRun.Produced)
            {
                scenarioRun.Log.LogInformation("Checking result of planned step {num}", planResult.Plan.Step);
                EventsExtensions.CompareEvents(planResult.Plan.ExpectedEvents, planResult.ProducedEvents);
            }
            return scenarioRun;
        }

       
        private static void LogDebugInfo<TAggregate>(IAggregateScenarioRun<TAggregate> scenarioRun) where TAggregate : IAggregate
        {
            var logger = scenarioRun.Log;
            logger.LogInformation("Given events:\r\n{@events}", scenarioRun.Scenario.GivenEvents);

            foreach (var planRun in scenarioRun.Produced)
            {
                logger.LogInformation("Step {num}:", planRun.Plan.Step);
                    
                logger.LogInformation("commands: {@cmd}", planRun.Plan.GivenCommands);

                logger.LogInformation("Produced events:\r\n{@events}", planRun.ProducedEvents);
                logger.LogInformation("Expected events:\r\n{@events}", planRun.Plan.ExpectedEvents);
            }
        }
    }
}