using System.Collections.Generic;
using GridDomain.Aggregates;
using Serilog;

namespace GridDomain.Scenarios {
    public interface IAggregateScenarioRun<out TAggregate> where TAggregate : IAggregate
    {
        TAggregate Aggregate { get; }
        IReadOnlyCollection<IDomainEvent> ProducedEvents { get; }
        IAggregateScenario Scenario { get; }
        ILogger Log { get; }
    }
}