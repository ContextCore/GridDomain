using System;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using Serilog;

namespace GridDomain.Scenarios.Runners
{
    public static class AggregateScenarioRunBuilderLocalRunnerExtensions
    {
        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate, TAggregateCommandsHandler>(this IAggregateScenarioRunBuilder builder, ILogger log = null) where TAggregate : class, IAggregate where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>,new()
        {
            return await Local(builder, CreateCommandsHandler<TAggregate, TAggregateCommandsHandler>(), log);
        }

        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate>(this IAggregateScenarioRunBuilder builder, ILogger log = null) where TAggregate : CommandAggregate
        {
            return await Local(builder, CommandAggregateHandler.New<TAggregate>(), log);
        }

        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate>(this IAggregateScenarioRunBuilder builder, IAggregateCommandsHandler<TAggregate> handler, ILogger log = null) where TAggregate : class, IAggregate
        {

            return await Local(builder, new AggregateFactory(), handler, log);
        }

        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate>(this IAggregateScenarioRunBuilder builder, IConstructAggregates aggregateFactory, IAggregateCommandsHandler<TAggregate> handler, ILogger log = null) where TAggregate : class, IAggregate
        {
            var id = builder.Scenario.GivenEvents.FirstOrDefault()?.SourceId
                          ?? builder.Scenario.GivenCommands.First().AggregateId 
                          ?? throw new CannotDetermineAggregateIdException();

            var runner = new AggregateScenarioLocalRunner<TAggregate>((TAggregate) aggregateFactory.Build(typeof(TAggregate), id), handler, log ?? Log.Logger);
            return await runner.Run(builder.Scenario);
        }

        private static IAggregateCommandsHandler<TAggregate> CreateCommandsHandler<TAggregate, THandler>() where THandler : IAggregateCommandsHandler<TAggregate> where TAggregate : IAggregate
        {
            var constructorInfo = typeof(THandler).GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
                throw new CannotCreateCommandHandlerExeption();

            return (IAggregateCommandsHandler<TAggregate>) constructorInfo.Invoke(null);
        }
    }
}