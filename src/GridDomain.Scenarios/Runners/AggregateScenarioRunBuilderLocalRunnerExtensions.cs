using System.Threading.Tasks;
using GridDomain.Aggregates;
using GridDomain.Scenarios.Builders;
using Microsoft.Extensions.Logging;

namespace GridDomain.Scenarios.Runners
{
    public static class AggregateScenarioRunBuilderLocalRunnerExtensions
    {
        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate>(this IAggregateScenarioRunBuilder<TAggregate> builder, ILoggerProvider log) where TAggregate : class, IAggregate
        {
            var runner = new AggregateScenarioLocalRunner<TAggregate>(log.CreateLogger(nameof(AggregateScenarioLocalRunner<TAggregate>)));
            return await runner.Run(builder.Scenario);
        }
    }
}