
using GridDomain.Aggregates;

namespace GridDomain.Scenarios.Builders {
    public interface IAggregateScenarioBuilder<T> where T : IAggregate
    {
        IAggregateScenario<T> Build();
        IAggregateScenarioBuilder<T> Given(params DomainEvent[] events);
        IAggregateScenarioBuilder<T> When(params Command[] commands);
        IAggregateScenarioBuilder<T> Then(params DomainEvent[] expectedEvents);
        IAggregateScenarioBuilder<T> With(IAggregateDependencies<T> dependencies);
        IAggregateScenarioBuilder<T> Name(string name);
        IAggregateScenarioRunBuilder<T> Run { get; }
    }
}