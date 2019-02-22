
using GridDomain.Aggregates;

namespace GridDomain.Scenarios.Builders {
    public interface IAggregateScenarioBuilder<T> where T : IAggregate
    {
        IAggregateScenario<T> Build();
        IAggregateScenarioBuilder<T> Given(params IDomainEvent[] events);
        IAggregateScenarioBuilder<T> When(params ICommand[] commands);
        IAggregateScenarioBuilder<T> Then(params IDomainEvent[] expectedEvents);
        IAggregateScenarioBuilder<T> With(IAggregateDependencies<T> dependencies);
        IAggregateScenarioBuilder<T> Name(string name);
        IAggregateScenarioRunBuilder<T> Run { get; }
    }
}