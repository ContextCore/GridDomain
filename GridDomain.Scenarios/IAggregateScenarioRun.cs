using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using Serilog;

namespace GridDomain.Scenarios {
    public interface IAggregateScenarioRun<out TAggregate> where TAggregate : IAggregate
    {
        TAggregate Aggregate { get; }
        IReadOnlyCollection<DomainEvent> ProducedEvents { get; }
        IAggregateScenario Scenario { get; }
        ILogger Log { get; }
    }
}