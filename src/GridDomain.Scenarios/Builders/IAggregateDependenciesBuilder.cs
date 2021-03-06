
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Scenarios.Builders {
    public interface IAggregateDependenciesBuilder<T> where T : IAggregate
    {
        IAggregateConfiguration<T> Build();
        IAggregateDependenciesBuilder<T> Constructor(IAggregateFactory<T> constructor);
        IAggregateScenarioBuilder<T> Scenario { get; }
    }
}