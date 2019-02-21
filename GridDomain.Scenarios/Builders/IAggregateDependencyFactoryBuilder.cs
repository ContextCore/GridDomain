
using GridDomain.Aggregates;

namespace GridDomain.Scenarios.Builders {
    public interface IAggregateDependenciesBuilder<T> where T : IAggregate
    {
        IAggregateDependencies<T> Build();
        IAggregateDependenciesBuilder<T> Constructor(IAggregateFactory<T> constructor);
        IAggregateScenarioBuilder<T> Scenario { get; }
    }
}