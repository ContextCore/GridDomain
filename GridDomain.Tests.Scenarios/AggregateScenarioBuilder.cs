using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Scenarios {
    public class AggregateScenarioBuilder:IAggregateScenarioBuilder
    {
        public static IAggregateScenarioBuilder New()
        {
            return new AggregateScenarioBuilder();
        }

        private DomainEvent[] _domainEvents={};
        private Command[] _commands={};
        private DomainEvent[] _exprectedEvents={};

        public IAggregateScenario Build()
        {
            return new AggregateScenario(_domainEvents,_commands,_exprectedEvents);
        }

        public IAggregateScenarioBuilder Given(params DomainEvent[] events)
        {
            _domainEvents = events;
            return this;
        }

        public IAggregateScenarioBuilder When(params Command[] commands)
        {
            _commands = commands;
            return this;
        }

        public IAggregateScenarioBuilder Then(params DomainEvent[] exprectedEvents)
        {
            _exprectedEvents = exprectedEvents;
            return this;
        }

        public IAggregateScenarioRunBuilder Run => new AggregateScenarioRunBuilder(Build());
    }
}