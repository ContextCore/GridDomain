
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Scenarios.Builders {
    public interface IAggregateScenarioBuilder<T> where T : IAggregate
    {
        IAggregateScenario<T> Build();
        IAggregateScenarioBuilder<T> Given(params IDomainEvent[] events);
        IAggregateScenarioBuilder<T> When(params ICommand[] commands);
        IAggregateScenarioBuilder<T> Then(params IDomainEvent[] expectedEvents);
        IAggregateScenarioBuilder<T> And();
        IAggregateScenarioBuilder<T> With(IAggregateConfiguration<T> configuration);
        IAggregateScenarioBuilder<T> Name(string name);
        IAggregateScenarioRunBuilder<T> Run { get; }
    }
}