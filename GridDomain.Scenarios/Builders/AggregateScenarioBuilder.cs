using GridDomain.Configuration;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Scenarios.Builders
{

    public class AggregateScenarioBuilder<T> : IAggregateScenarioBuilder<T> where T : IAggregate
    {
        private DomainEvent[] _domainEvents = { };
        private Command[] _commands = { };
        private DomainEvent[] _expectedEvents = { };
        private IAggregateDependencies<T> _aggregateDependencies;

        public IAggregateScenario<T> Build()
        {
            return new AggregateScenario<T>(_domainEvents, _commands, _expectedEvents, _aggregateDependencies ?? new AggregateDependenciesBuilder<T>(this).Build());
        }

        public IAggregateScenarioBuilder<T> Given(params DomainEvent[] events)
        {
            _domainEvents = events;
            return this;
        }

        public IAggregateScenarioBuilder<T> When(params Command[] commands)
        {
            _commands = commands;
            return this;
        }

        public IAggregateScenarioBuilder<T> Then(params DomainEvent[] exprectedEvents)
        {
            _expectedEvents = exprectedEvents;
            return this;
        }

        public IAggregateScenarioBuilder<T> With(IAggregateDependencies<T> factory)
        {
            _aggregateDependencies = factory;
            return this;
        }

        public IAggregateScenarioRunBuilder<T> Run => Build().Run();
    }

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

        public static IAggregateScenarioBuilder<T> With<T>(this IAggregateScenarioBuilder<T> builder, IAggregateCommandsHandler<T> handler) where T : IAggregate
        {
             var dependenciesBuilder = new AggregateDependenciesBuilder<T>(builder);
            dependenciesBuilder.Handler(handler);
            return builder.With(dependenciesBuilder.Build());
        }

        public static IAggregateScenarioBuilder<T> With<T>(this IAggregateScenarioBuilder<T> builder, IAggregateFactory factory) where T : IAggregate
        {
            var dependenciesBuilder = new AggregateDependenciesBuilder<T>(builder);
            dependenciesBuilder.Constructor(factory);
            return builder.With(dependenciesBuilder.Build());
        }

    }
}