using System.Reflection;
using System.Runtime.CompilerServices;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Scenarios.Builders
{
    class AggregateDependenciesBuilder<T> : IAggregateDependenciesBuilder<T> where T : IAggregate
    {
        private IAggregateCommandsHandler<T> _aggregateCommandsHandler;
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

            var aggregateDependencies = new AggregateDependencies<T>(() => _aggregateCommandsHandler ?? CreateCommandsHandler(_aggregateFactory))
                                        {
                                            AggregateFactoryCreator = () => _aggregateFactory
                                        };
            return aggregateDependencies;
        }

        private static IAggregateCommandsHandler<T> CreateCommandsHandler(IAggregateFactory factory)
        {
            if (typeof(CommandAggregate).IsAssignableFrom(typeof(T)))
            {
                var methodOpenType = typeof(CommandAggregateHandler).GetTypeInfo()
                                                                    .GetMethod(nameof(CommandAggregateHandler.New));

                var commandAggregateHandler = methodOpenType.MakeGenericMethod(typeof(T))
                                                            .Invoke(null, new object[]{factory});

                return (IAggregateCommandsHandler<T>) commandAggregateHandler;
            }

            return new AggregateCommandsHandler<T>();
        }

        public IAggregateDependenciesBuilder<T> Handler(IAggregateCommandsHandler<T> handler)
        {
            _aggregateCommandsHandler = handler;
            return this;
        }

        public IAggregateDependenciesBuilder<T> Constructor(IAggregateFactory constructor)
        {
            _aggregateFactory = constructor;
            return this;
        }
    }
}