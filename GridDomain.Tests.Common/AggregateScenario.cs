using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Tools;

namespace GridDomain.Tests.Common
{

    public static class AggregateScenario
    {
        public static AggregateScenario<TAggregate> New<TAggregate,TAggregateCommandsHandler>() where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
                                                                                                where TAggregate : Aggregate
        {
            return new AggregateScenario<TAggregate>(CreateAggregate<TAggregate>(), CreateCommandsHandler<TAggregate,TAggregateCommandsHandler>());
        }

        public static AggregateScenario<TAggregate> New<TAggregate>() where TAggregate : CommandAggregate
        {
            return new AggregateScenario<TAggregate>(CreateAggregate<TAggregate>(), CommandAggregateHandler.New<TAggregate>());
        }

        public static AggregateScenario<TAggregate> New<TAggregate>(IAggregateCommandsHandler<TAggregate> handler) where TAggregate : Aggregate
        {
            return new AggregateScenario<TAggregate>(CreateAggregate<TAggregate>(), handler);
        }
        private static TAggregate CreateAggregate<TAggregate>() where TAggregate: Aggregate
        {
            return (TAggregate)new AggregateFactory().Build(typeof(TAggregate), Guid.NewGuid(), null);
        }

        private static IAggregateCommandsHandler<TAggregate> CreateCommandsHandler<TAggregate,THandler>() where THandler : IAggregateCommandsHandler<TAggregate> where TAggregate : IAggregate
        {
            var constructorInfo = typeof(THandler).GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
                throw new CannotCreateCommandHandlerExeption();

            return (IAggregateCommandsHandler<TAggregate>)constructorInfo.Invoke(null);
        }

    }

    public class AggregateScenario<TAggregate> where TAggregate : Aggregate
    {
        public AggregateScenario(TAggregate aggregate, IAggregateCommandsHandler<TAggregate> handler)
        {
            CommandsHandler = handler ?? throw new ArgumentNullException(nameof(handler));
            Aggregate = aggregate ?? throw new ArgumentNullException(nameof(aggregate));
            Aggregate.InitEventStore(_eventStore);
        }

        private IAggregateCommandsHandler<TAggregate> CommandsHandler { get; }
        public TAggregate Aggregate { get; private set; }

        public DomainEvent[] ExpectedEvents { get; private set; } = {};
        public DomainEvent[] ProducedEvents { get; private set; } = {};
        public DomainEvent[] GivenEvents { get; private set; } = {};
        public Command[] GivenCommands { get; private set; } = {};
        private readonly InMemoryEventStore _eventStore = new InMemoryEventStore();
        public AggregateScenario<TAggregate> Given(params DomainEvent[] events)
        {
            GivenEvents = events;
            Aggregate.ApplyEvents(events);
            return this;
        }

        public AggregateScenario<TAggregate> When(params Command[] commands)
        {
            GivenCommands = commands;
            return this;
        }

        public AggregateScenario<TAggregate> Then(params DomainEvent[] expectedEvents)
        {
            ExpectedEvents = expectedEvents;
            return this;
        }

        public async Task<AggregateScenario<TAggregate>> Run()
        {
            //When
            foreach (var cmd in GivenCommands)
            {
                try
                {
                    Aggregate = await CommandsHandler.ExecuteAsync(Aggregate, cmd, _eventStore);
                }
                catch (Exception ex)
                {
                    throw new CommandExecutionFailedException(cmd,ex);
                }
                Aggregate.CommitAll();
            }

            //Then
            ProducedEvents = _eventStore.Events.ToArray();
            _eventStore.Clear();

            return this;
        }
    }
}