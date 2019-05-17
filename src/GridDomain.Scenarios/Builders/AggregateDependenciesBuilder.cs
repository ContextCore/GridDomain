
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Scenarios.Builders
{
    class AggregateDependenciesBuilder<T> : IAggregateDependenciesBuilder<T> where T : IAggregate
    {
        private IAggregateFactory<T> _aggregateFactory;
        private readonly IAggregateScenarioBuilder<T> _builder;

        public AggregateDependenciesBuilder(IAggregateScenarioBuilder<T> builder)
        {
            _builder = builder;
        }

        public IAggregateScenarioBuilder<T> Scenario => _builder.With(Build());

        public IAggregateConfiguration<T> Build()
        {
           return new AggregateConfiguration<T>(_aggregateFactory);
        }

        public IAggregateDependenciesBuilder<T> Handler(object handler)
        {
            return this;
        }

        public IAggregateDependenciesBuilder<T> Constructor(IAggregateFactory<T> constructor)
        {
            _aggregateFactory = constructor;
            return this;
        }
    }
}