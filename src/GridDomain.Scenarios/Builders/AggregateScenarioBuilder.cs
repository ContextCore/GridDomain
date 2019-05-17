using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.Scenarios.Builders
{

    public class AggregateScenarioBuilder<T> : IAggregateScenarioBuilder<T> where T : IAggregate
    {
        private IDomainEvent[] _domainEvents = { };
        private IAggregateConfiguration<T> _aggregateConfiguration;
        private string _name = "AggregateScenario_"+typeof(T).Name;
        private readonly List<Plan> _plans = new List<Plan>();

        private int _step = 1;
        private readonly List<ICommand> _currentPlanCommands = new List<ICommand>();
        private readonly List<IDomainEvent> _currentPlanEvents = new List<IDomainEvent>();
        public IAggregateScenario<T> Build()
        {
            if (_currentPlanEvents.Any() || _currentPlanCommands.Any())
                And();
            
            return new AggregateScenario<T>(_domainEvents, _plans, _aggregateConfiguration ?? new AggregateDependenciesBuilder<T>(this).Build(),_name);
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
            _currentPlanCommands.AddRange(commands);
            return this;
        }

        public IAggregateScenarioBuilder<T> Then(params IDomainEvent[] exprectedEvents)
        {
            _currentPlanEvents.AddRange(exprectedEvents);
            return this;
        }
        
        public IAggregateScenarioBuilder<T> And()
        {
            _plans.Add(new Plan(_currentPlanCommands.ToArray(), _currentPlanEvents.ToArray(), _step++));
            _currentPlanCommands.Clear();
            _currentPlanEvents.Clear();
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