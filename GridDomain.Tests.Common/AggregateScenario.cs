using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Tests.Common
{

    public static class AggregateScenario
    {
        public static AggregateScenario<TAggregate> New<TAggregate,TAggregateCommandsHandler>() where TAggregateCommandsHandler : IAggregateCommandsHandler<TAggregate>
                                                                                                where TAggregate : Aggregate
        {
            return new AggregateScenario<TAggregate>(CreateAggregate<TAggregate>(), CreateCommandsHandler<TAggregate,TAggregateCommandsHandler>());
        }

        public static AggregateScenario<TAggregate> New<TAggregate>(IAggregateCommandsHandler<TAggregate> handler) where TAggregate : Aggregate
        {
            return new AggregateScenario<TAggregate>(CreateAggregate<TAggregate>(), handler);
        }
        private static TAggregate CreateAggregate<TAggregate>() where TAggregate: Aggregate
        {
            return (TAggregate)new AggregateFactory().Build(typeof(TAggregate), Guid.NewGuid(), null);
        }

        private static IAggregateCommandsHandler<TAggregate> CreateCommandsHandler<TAggregate,THandler>() where THandler : IAggregateCommandsHandler<TAggregate>
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
        }

        private IAggregateCommandsHandler<TAggregate> CommandsHandler { get; }
        public TAggregate Aggregate { get; private set; }

        public DomainEvent[] ExpectedEvents { get; private set; } = {};
        public DomainEvent[] ProducedEvents { get; private set; } = {};
        public DomainEvent[] GivenEvents { get; private set; } = {};
        public Command[] GivenCommands { get; private set; } = {};

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
            var events = new List<DomainEvent>();
            Task Persistence(Aggregate agr)
            {
                Aggregate = (TAggregate)agr;
                events.AddRange(((IAggregate)Aggregate).GetUncommittedEvents().ToList());
                agr.PersistAll();
                return Task.CompletedTask;
            }

            //When
            foreach (var cmd in GivenCommands)
            {
               await CommandsHandler.ExecuteAsync(Aggregate, cmd, Persistence);
            }

            //Then
            ProducedEvents = events.ToArray();

            return this;
        }
    }
}