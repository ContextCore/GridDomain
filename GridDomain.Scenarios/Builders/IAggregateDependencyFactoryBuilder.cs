using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Scenarios.Builders {
    public interface IAggregateDependenciesBuilder<T> where T : IAggregate
    {
        IAggregateDependencies<T> Build();
        IAggregateDependenciesBuilder<T> Handler(IAggregateCommandsHandler<T> handler);
        IAggregateDependenciesBuilder<T> Constructor(IAggregateFactory constructor);
        IAggregateScenarioBuilder<T> Scenario { get; }
    }
}