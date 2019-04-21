using System;
using GridDomain.Aggregates;

namespace GridDomain.Scenarios.Builders
{

    public class AggregateScenarioBuilder<T> : IAggregateScenarioBuilder<T> where T : IAggregate
    {
        private IDomainEvent[] _domainEvents = { };
        private ICommand[] _commands = { };
        private IDomainEvent[] _expectedEvents = { };
        private IAggregateConfiguration<T> _aggregateConfiguration;
        private string _name = "AggregateScenario_"+typeof(T).Name;

        public IAggregateScenario<T> Build()
        {
            return new AggregateScenario<T>(_domainEvents, _commands, _expectedEvents, _aggregateConfiguration ?? new AggregateDependenciesBuilder<T>(this).Build(),_name);
        }

        public IAggregateScenarioBuilder<T> Name(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidScenarioNameException();

            _name = name;
            return this;
        }

        public class InvalidScenarioNameException : Exception { }

        public IAggregateScenarioBuilder<T> Given(params IDomainEvent[] events)
        {
            _domainEvents = events;
            return this;
        }

        public IAggregateScenarioBuilder<T> When(params ICommand[] commands)
        {
            _commands = commands;
            return this;
        }

        public IAggregateScenarioBuilder<T> Then(params IDomainEvent[] exprectedEvents)
        {
            _expectedEvents = exprectedEvents;
            return this;
        }

        public IAggregateScenarioBuilder<T> With(IAggregateConfiguration<T> factory)
        {
            _aggregateConfiguration = factory;
            return this;
        }

        public IAggregateScenarioRunBuilder<T> Run => Build().Run();
    }
}