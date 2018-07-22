using GridDomain.CQRS;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Scenarios {
    public interface IAggregateScenarioBuilder
    {
        IAggregateScenario Build();
        IAggregateScenarioBuilder Given(params DomainEvent[] events);
        IAggregateScenarioBuilder When(params Command[] commands);
        IAggregateScenarioBuilder Then(params DomainEvent[] expectedEvents);
        IAggregateScenarioRunBuilder Run { get; }
    }
}