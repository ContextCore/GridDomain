using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Tests.Common;
using Serilog;
using Serilog.Core;

namespace GridDomain.Tests.Scenarios
{
    public class AggregateScenarioRunBuilder: IAggregateScenarioRunBuilder
    {
        public IAggregateScenarioBuilder<IAggregateScenarioRunBuilder> Scenario { get; }
        public IAggregateScenarioRunnerBuilder Run { get; }
    }

    public class AggregateScenario:IAggregateScenario
    {
        public AggregateScenario(IReadOnlyCollection<DomainEvent> givenEvents, 
                                 IReadOnlyCollection<ICommand> givenCommands, 
                                 IReadOnlyCollection<DomainEvent> expectedEvents)
        {
            GivenEvents = givenEvents;
            GivenCommands = givenCommands;
            ExpectedEvents = expectedEvents;


            if (GivenCommands.Any())
            {
                var aggregateName = GivenCommands.First().AggregateType;

                if (GivenCommands.Any(c => c.AggregateType != aggregateName))
                    throw new CommandsBelongsToDifferentAggregatesException(GivenCommands);
            }
        }
        public IReadOnlyCollection<DomainEvent> ExpectedEvents { get; }
        public IReadOnlyCollection<DomainEvent> GivenEvents { get; }
        public IReadOnlyCollection<ICommand> GivenCommands { get; }
    }

    public class CommandsBelongsToDifferentAggregatesException : Exception
    {
        public IReadOnlyCollection<ICommand> Commands { get; }

        public CommandsBelongsToDifferentAggregatesException(IReadOnlyCollection<ICommand> commands)
        {
            Commands = commands;
        }
    }

    public class AggregateScenarioBuilder : IAggregateScenarioBuilder
    {
        private DomainEvent[] _domainEvents;
        private Command[] _commands;
        private DomainEvent[] _exprectedEvents;

        public IAggregateScenario Build()
        {
           return new AggregateScenario(_domainEvents,_commands,_exprectedEvents);
        }

        public IAggregateScenarioBuilder Given(params DomainEvent[] events)
        {
            _domainEvents = events;
            return this;
        }

        public IAggregateScenarioBuilder When(params Command[] commands)
        {
            _commands = commands;
            return this;
        }

        public IAggregateScenarioBuilder Then(params DomainEvent[] exprectedEvents)
        {
            _exprectedEvents = exprectedEvents;
            return this;
        }
    }

   
    public class AggregateScenarioRun<TAggregate> : IAggregateScenarioRun<TAggregate> where TAggregate : IAggregate
    {
        public AggregateScenarioRun(IAggregateScenario scenario, TAggregate aggregate, IReadOnlyCollection<DomainEvent> producedEvents, ILogger log)
        {
            Aggregate = aggregate;
            ProducedEvents = producedEvents;
            Log = log;
            Scenario = scenario;
        }
        public TAggregate Aggregate { get; }
        public IReadOnlyCollection<DomainEvent> ProducedEvents { get; }
        public IAggregateScenario Scenario { get; }
        public ILogger Log { get; }
    }


    //public static class AggregateScenarioBuilderLocalRunnerExtensions
    //{
    //    public static async Task<IAggregateScenarioRun<TAggregate>> RunLocal<TAggregate,TAggregateCommandsHandler>(this IAggregateScenarioBuilder builder) where TAggregate : class, IAggregate where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
    //    {
    //        var runner = AggregateScenarioLocalRunner.New<TAggregate, TAggregateCommandsHandler>();
    //        return await runner.Run(builder.Build());
    //    }
    //
    //    public static async Task<IAggregateScenarioRun<TAggregate>> RunLocal<TAggregate>(this IAggregateScenarioBuilder builder) where TAggregate : CommandAggregate
    //    {
    //        var runner = AggregateScenarioLocalRunner.New<TAggregate>();
    //        return await runner.Run(builder.Build());
    //    }
    //
    //    public static async Task<IAggregateScenarioRun<TAggregate>> RunLocal<TAggregate>(this IAggregateScenarioBuilder builder,IAggregateCommandsHandler<TAggregate> handler) where TAggregate : class, IAggregate
    //    {
    //        var runner = AggregateScenarioLocalRunner.New(handler);
    //        return await runner.Run(builder.Build());
    //    }
    //}

    public static class AggregateScenarioRunBuilderLocalRunnerExtensions
    {
        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate,TAggregateCommandsHandler>(this IAggregateScenarioRunBuilder builder) where TAggregate : class, IAggregate where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
        {
            var runner = AggregateScenarioLocalRunner.New<TAggregate, TAggregateCommandsHandler>();
            return await runner.Run(builder.Scenario.Build());
        }

        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate>(this IAggregateScenarioRunBuilder builder) where TAggregate : CommandAggregate
        {
            var runner = AggregateScenarioLocalRunner.New<TAggregate>();
            return await runner.Run(builder.Scenario.Build());
        }

        public static async Task<IAggregateScenarioRun<TAggregate>> Local<TAggregate>(this IAggregateScenarioRunBuilder builder,IAggregateCommandsHandler<TAggregate> handler) where TAggregate : class, IAggregate
        {
            var runner = AggregateScenarioLocalRunner.New(handler);
            return await runner.Run(builder.Scenario.Build());
        }
    }


    public static class AggregateScenarioLocalRunner
    {
        public static AggregateScenarioLocalRunner<TAggregate> New<TAggregate,TAggregateCommandsHandler>() where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
                                                                                                           where TAggregate : class, IAggregate
        {
            return new AggregateScenarioLocalRunner<TAggregate>(CreateAggregate<TAggregate>(), CreateCommandsHandler<TAggregate,TAggregateCommandsHandler>());
        }

        public static AggregateScenarioLocalRunner<TAggregate> New<TAggregate>() where TAggregate : CommandAggregate
        {
            return new AggregateScenarioLocalRunner<TAggregate>(CreateAggregate<TAggregate>(), CommandAggregateHandler.New<TAggregate>());
        }

        public static AggregateScenarioLocalRunner<TAggregate> New<TAggregate>(IAggregateCommandsHandler<TAggregate> handler) where TAggregate : class, IAggregate
        {
            return new AggregateScenarioLocalRunner<TAggregate>(CreateAggregate<TAggregate>(), handler);
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

    public class AggregateScenarioLocalRunner<TAggregate>: IAggregateScenarioRunner<TAggregate> where TAggregate : class,IAggregate
    {
        private TAggregate _aggregate;
        public ILogger Log { get; }

        public AggregateScenarioLocalRunner(TAggregate aggregate, IAggregateCommandsHandler<TAggregate> handler, ILogger log = null)
        {
            CommandsHandler = handler ?? throw new ArgumentNullException(nameof(handler));
            _aggregate = aggregate ?? throw new ArgumentNullException(nameof(aggregate));
            Log = log ?? Serilog.Log.Logger;
        }

        private IAggregateCommandsHandler<TAggregate> CommandsHandler { get; }
        public async Task<IAggregateScenarioRun<TAggregate>> Run(IAggregateScenario scenario)
        {
            //When
            foreach (var cmd in scenario.GivenCommands)
            {
                try
                {
                    _aggregate = await CommandsHandler.ExecuteAsync(_aggregate, cmd);
                }
                catch (Exception ex)
                {
                    var commandExecutionFailedException = new CommandExecutionFailedException(cmd,ex);
                    Log.Error(commandExecutionFailedException,"failed to execute an aggregate command");
                    throw commandExecutionFailedException;
                }
            }

            //Then
            var producedEvents = _aggregate.GetUncommittedEvents().ToArray();
            _aggregate.ClearUncommitedEvents();
            return new AggregateScenarioRun<TAggregate>(scenario,_aggregate,producedEvents,Log);
        }

    }
}