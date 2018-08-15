using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Scenarios.Builders;
using Serilog;

namespace GridDomain.Scenarios.Runners
{
    public static class AggregateScenarioRunBuilderLocalRunnerExtensions
    {
//        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate, TAggregateCommandsHandler>(this IAggregateScenarioRunBuilder<TAggregate> builder, ILogger log = null) where TAggregate : class, IAggregate where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>,new()
//        {
//            return await Local(builder, CreateCommandsHandler<TAggregate, TAggregateCommandsHandler>(), log);
//        }
//
//        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate>(this IAggregateScenarioRunBuilder<TAggregate> builder, ILogger log = null) where TAggregate : CommandAggregate
//        {
//            return await Local(builder, CommandAggregateHandler.New<TAggregate>(), log);
//        }
//
//        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate>(this IAggregateScenarioRunBuilder<TAggregate> builder, IAggregateCommandsHandler<TAggregate> handler, ILogger log = null) where TAggregate : class, IAggregate
//        {
//
//            return await Local(builder, handler, log);
//        }

        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate>(this IAggregateScenarioRunBuilder<TAggregate> builder, ILogger log = null) where TAggregate : class, IAggregate
        {
            var runner = new AggregateScenarioLocalRunner<TAggregate>(log ?? Log.Logger);
            return await runner.Run(builder.Scenario);
        }

        private static IAggregateCommandsHandler<TAggregate> CreateCommandsHandler<TAggregate, THandler>() where THandler : IAggregateCommandsHandler<TAggregate> where TAggregate : IAggregate
        {
            var constructorInfo = typeof(THandler).GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
                throw new CannotCreateCommandHandlerException();

            return (IAggregateCommandsHandler<TAggregate>) constructorInfo.Invoke(null);
        }
    }
}