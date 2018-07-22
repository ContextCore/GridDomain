using System;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using Serilog;

namespace GridDomain.Tests.Scenarios.Runners
{
    public static class LocalRunner
    {
        public static AggregateScenarioLocalRunner<TAggregate> New<TAggregate,TAggregateCommandsHandler>(ILogger log) where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
                                                                                                           where TAggregate : class, IAggregate
        {
            return new AggregateScenarioLocalRunner<TAggregate>(CreateAggregate<TAggregate>(), CreateCommandsHandler<TAggregate,TAggregateCommandsHandler>(),log);
        }

        public static AggregateScenarioLocalRunner<TAggregate> New<TAggregate>(ILogger log) where TAggregate : CommandAggregate
        {
            return new AggregateScenarioLocalRunner<TAggregate>(CreateAggregate<TAggregate>(), CommandAggregateHandler.New<TAggregate>(),log);
        }

        public static AggregateScenarioLocalRunner<TAggregate> New<TAggregate>(IAggregateCommandsHandler<TAggregate> handler,ILogger log) where TAggregate : class, IAggregate
        {
            return new AggregateScenarioLocalRunner<TAggregate>(CreateAggregate<TAggregate>(), handler, log);
        }

        private static TAggregate CreateAggregate<TAggregate>() where TAggregate: IAggregate
        {
            return (TAggregate)new AggregateFactory().Build(typeof(TAggregate), Guid.NewGuid().ToString(), null);
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