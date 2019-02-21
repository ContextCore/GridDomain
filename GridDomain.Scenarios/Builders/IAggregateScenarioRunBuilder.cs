
using GridDomain.Aggregates;

namespace GridDomain.Scenarios.Builders {
   
    public interface IAggregateScenarioRunBuilder<T> where T : IAggregate
    {
         IAggregateScenario<T> Scenario { get; }
    }
}