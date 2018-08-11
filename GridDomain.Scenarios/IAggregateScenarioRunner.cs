using System.Threading.Tasks;
using GridDomain.EventSourcing.CommonDomain;
using Serilog;

namespace GridDomain.Tests.Scenarios {
    public interface IAggregateScenarioRunner<TAggregate> where TAggregate : IAggregate
    {
        Task<IAggregateScenarioRun<TAggregate>> Run(IAggregateScenario scenario);
        ILogger Log { get; }
    }
}