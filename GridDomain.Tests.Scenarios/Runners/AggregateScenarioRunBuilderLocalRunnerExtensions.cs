using System.Linq;
using System.Threading.Tasks;
using GridDomain.Configuration;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using Serilog;

namespace GridDomain.Tests.Scenarios.Runners
{
    public static class AggregateScenarioRunBuilderLocalRunnerExtensions
    {
        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate, TAggregateCommandsHandler>(this IAggregateScenarioRunBuilder builder, ILogger log) where TAggregate : class, IAggregate where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
        {
            var runner = LocalRunner.New<TAggregate, TAggregateCommandsHandler>(log);
            return await runner.Run(builder.Scenario);
        }

        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate>(this IAggregateScenarioRunBuilder builder, ILogger log) where TAggregate : CommandAggregate
        {
            var runner = LocalRunner.New<TAggregate>(log);
            return await runner.Run(builder.Scenario);
        }

        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate>(this IAggregateScenarioRunBuilder builder, IConstructAggregates aggregateFactory,  IAggregateCommandsHandler<TAggregate> handler, ILogger log) where TAggregate : class, IAggregate
        {
            var aggregareId = builder.Scenario.GivenEvents.FirstOrDefault()
                                     ?.SourceId
                              ?? builder.Scenario.GivenCommands.First()
                                        .AggregateId;

            var runner = new AggregateScenarioLocalRunner<TAggregate>((TAggregate)aggregateFactory.Build(typeof(TAggregate),aggregareId), handler, log);
            return await runner.Run(builder.Scenario);
        }
    }
}