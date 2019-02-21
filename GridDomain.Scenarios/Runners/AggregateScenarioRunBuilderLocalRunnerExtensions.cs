using System.Threading.Tasks;
using GridDomain.Aggregates;
using GridDomain.Scenarios.Builders;
using Serilog;

namespace GridDomain.Scenarios.Runners
{
    public static class AggregateScenarioRunBuilderLocalRunnerExtensions
    {
        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate>(this IAggregateScenarioRunBuilder<TAggregate> builder, ILogger log = null) where TAggregate : class, IAggregate
        {
            var runner = new AggregateScenarioLocalRunner<TAggregate>(log ?? Log.Logger);
            return await runner.Run(builder.Scenario);
        }
    }
}