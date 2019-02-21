using System.Threading.Tasks;
using GridDomain.Aggregates;
using Serilog;

namespace GridDomain.Scenarios {
    public interface IAggregateScenarioRunner<TAggregate> where TAggregate : IAggregate
    {
        Task<IAggregateScenarioRun<TAggregate>> Run(IAggregateScenario<TAggregate> scenario);
        ILogger Log { get; }
    }
}