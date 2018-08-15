using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Scenarios.Builders;

namespace GridDomain.Scenarios {
    public static class AggregateScenarioExtensions
    {
        public static IAggregateScenarioRunBuilder<T> Run<T>(this IAggregateScenario<T> scenario) where T : IAggregate
        {
            return new AggregateScenarioRunBuilder<T>(scenario);
        }
    }
}