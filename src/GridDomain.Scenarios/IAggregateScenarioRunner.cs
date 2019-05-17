using System.Threading.Tasks;
using GridDomain.Aggregates;
using GridDomain.Aggregates.Abstractions;
using Microsoft.Extensions.Logging;

namespace GridDomain.Scenarios {
    public interface IAggregateScenarioRunner<TAggregate> where TAggregate : IAggregate
    {
        Task<IAggregateScenarioRun<TAggregate>> Run(IAggregateScenario<TAggregate> scenario);
        ILogger Log { get; }
    }
}