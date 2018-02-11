using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.Tests.Common;

namespace GridDomain.Tests.Scenarios {
    public static class AggregateScenarioExtensions
    {

        public static async Task<AggregateScenario<TAggregate>> Check<TAggregate>(this Task<AggregateScenario<TAggregate>> scenario) where TAggregate : Aggregate
        {
            var sc = await scenario;
            return sc.Check();
        }

        public static AggregateScenario<TAggregate> Check<TAggregate>(this AggregateScenario<TAggregate> scenario) where TAggregate : Aggregate
        {
            LogDebugInfo(scenario);
            EventsExtensions.CompareEvents(scenario.ExpectedEvents, scenario.ProducedEvents);
            return scenario;
        }

        private static void LogDebugInfo<TAggregate>(AggregateScenario<TAggregate> sc) where TAggregate : Aggregate
        {
            var logger = sc.Log;
            foreach (var cmd in sc.GivenCommands)
                sc.Log.Information("Command: {@cmd}", cmd);

            logger.Information("Given events:\r\n{@events}",  sc.GivenEvents);
            logger.Information("Produced events:\r\n{@events}",  sc.ProducedEvents);
            logger.Information("Expected events:\r\n{@events}",  sc.ExpectedEvents);
        }
    }
}