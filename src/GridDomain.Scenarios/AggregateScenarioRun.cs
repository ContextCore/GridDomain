using System.Collections.Generic;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;
using Microsoft.Extensions.Logging;

namespace GridDomain.Scenarios {
    public class AggregateScenarioRun<TAggregate> : IAggregateScenarioRun<TAggregate> where TAggregate : IAggregate
    {
        public AggregateScenarioRun(IAggregateScenario scenario, TAggregate aggregate, IReadOnlyCollection<PlanRun> producedEvents, ILogger log)
        {
            Aggregate = aggregate;
            Produced = producedEvents ?? new PlanRun[]{};
            Log = log;
            Scenario = scenario;
        }
        public TAggregate Aggregate { get; }
        public IReadOnlyCollection<PlanRun> Produced { get; }
        public IAggregateScenario Scenario { get; }
        public ILogger Log { get; }
    }
}