using System.Reflection;
using System.Runtime.CompilerServices;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Scenarios.Builders
{
    class AggregateDependenciesBuilder<T> : IAggregateDependenciesBuilder<T> where T : IAggregate
    {
        private IAggregateFactory _aggregateFactory;
        private readonly IAggregateScenarioBuilder<T> _builder;

        public AggregateDependenciesBuilder(IAggregateScenarioBuilder<T> builder)
        {
            _builder = builder;
        }

        public IAggregateScenarioBuilder<T> Scenario => _builder.With(Build());

        public IAggregateDependencies<T> Build()
        {
            _aggregateFactory = _aggregateFactory ?? new AggregateFactory();

            var aggregateDependencies = new AggregateDependencies<T>()
                                        {
                                            AggregateFactory = _aggregateFactory
                                        };
            return aggregateDependencies;
        }

        public IAggregateDependenciesBuilder<T> Handler(object handler)
        {
            return this;
        }

        public IAggregateDependenciesBuilder<T> Constructor(IAggregateFactory constructor)
        {
            _aggregateFactory = constructor;
            return this;
        }
    }
}