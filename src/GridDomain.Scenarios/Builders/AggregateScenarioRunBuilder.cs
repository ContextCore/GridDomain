
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Scenarios.Builders {

    public class AggregateScenarioRunBuilder<T>: IAggregateScenarioRunBuilder<T> where T : IAggregate
    {
        public AggregateScenarioRunBuilder(IAggregateScenario<T> builder)
        {
            Scenario = builder;
        }
        public IAggregateScenario<T> Scenario { get; }
    }
}