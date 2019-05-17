using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Scenarios.Builders
{
    public static class AggregateScenarioBuilderExtensions
    {

        public static IAggregateDependenciesBuilder<T> With<T>(this IAggregateScenarioBuilder<T> builder) where T : IAggregate
        {
            return new AggregateDependenciesBuilder<T>(builder);
        }

        public static IAggregateDependenciesBuilder<T> With<T,U>(this IAggregateScenarioBuilder<T> builder) where T : IAggregate
        {
            return new AggregateDependenciesBuilder<T>(builder);
        }
        
        public static IAggregateScenarioBuilder<T> With<T>(this IAggregateScenarioBuilder<T> builder, IAggregateFactory<T> factory) where T : IAggregate
        {
            var dependenciesBuilder = new AggregateDependenciesBuilder<T>(builder);
            dependenciesBuilder.Constructor(factory);
            return builder.With(dependenciesBuilder.Build());
        }

    }
}