using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Scenarios.Builders {
   
    public interface IAggregateScenarioRunBuilder<T> where T : IAggregate
    {
         IAggregateScenario<T> Scenario { get; }
    }
}