using System.Collections.Generic;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using Serilog;

namespace GridDomain.Scenarios {
    public class AggregateScenarioRun<TAggregate> : IAggregateScenarioRun<TAggregate> where TAggregate : IAggregate
    {
        public AggregateScenarioRun(IAggregateScenario scenario, TAggregate aggregate, IReadOnlyCollection<DomainEvent> producedEvents, ILogger log)
        {
            Aggregate = aggregate;
            ProducedEvents = producedEvents ?? new DomainEvent[]{};
            Log = log;
            Scenario = scenario;
        }
        public TAggregate Aggregate { get; }
        public IReadOnlyCollection<DomainEvent> ProducedEvents { get; }
        public IAggregateScenario Scenario { get; }
        public ILogger Log { get; }
    }
}