using System.Collections.Generic;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using Serilog;

namespace GridDomain.Tests.Scenarios 
{
    public interface IAggregateScenario
    {
        IReadOnlyCollection<DomainEvent> ExpectedEvents { get; }
        IReadOnlyCollection<DomainEvent> GivenEvents { get; }
        IReadOnlyCollection<ICommand> GivenCommands { get; }
    }

    public interface IAggregateScenarioBuilder
    {
        IAggregateScenario Build();
        IAggregateScenarioBuilder Given(params DomainEvent[] events);
        IAggregateScenarioBuilder When(params Command[] commands);
        IAggregateScenarioBuilder Then(params DomainEvent[] expectedEvents);
    }

    public interface IAggregateScenarioBuilder<T>
    {
        IAggregateScenario Build();
        IAggregateScenarioBuilder<T> Given(params DomainEvent[] events);
        IAggregateScenarioBuilder<T> When(params Command[] commands);
        IAggregateScenarioBuilder<T> Then(params DomainEvent[] expectedEvents);
        T Run { get; }
    }

    public interface IAggregateScenarioRun<out TAggregate> where TAggregate : IAggregate
    {
        TAggregate Aggregate { get; }
        IReadOnlyCollection<DomainEvent> ProducedEvents { get; }
        IAggregateScenario Scenario { get; }
        ILogger Log { get; }
    }

    public interface IAggregateScenarioRunner<TAggregate> where TAggregate : IAggregate
    {
        Task<IAggregateScenarioRun<TAggregate>> Run(IAggregateScenario scenario);
        ILogger Log { get; }
    }

    public interface IAggregateScenarioRunBuilder
    {
        IAggregateScenarioBuilder<IAggregateScenarioRunBuilder> Scenario { get; }
    }

    public interface IAggregateScenarioRunnerBuilder
    {
        IAggregateScenarioBuilder ScenarioBuilder { get; }
    }
}