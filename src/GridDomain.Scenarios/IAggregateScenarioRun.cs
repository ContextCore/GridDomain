using System.Collections.Generic;
using GridDomain.Aggregates;
using Microsoft.Extensions.Logging;

namespace GridDomain.Scenarios {
    public interface IAggregateScenarioRun<out TAggregate> where TAggregate : IAggregate
    {
        TAggregate Aggregate { get; }
        IReadOnlyCollection<PlanRun> Produced { get; }
        IAggregateScenario Scenario { get; }
        ILogger Log { get; }
    }

    public class PlanRun
    {
        public PlanRun(Plan plan, IReadOnlyCollection<IDomainEvent> producedEvents)
        {
            Plan = plan;
            ProducedEvents = producedEvents;
        }

        public Plan Plan { get; }
        public IReadOnlyCollection<IDomainEvent> ProducedEvents { get; }
    }
}