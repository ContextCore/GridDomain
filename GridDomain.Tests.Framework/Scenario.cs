using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Logging;

namespace GridDomain.Tests.Framework
{
    public class Scenario
    {
        public static Scenario<TAggregate, TCommandsHandler> New<TAggregate, TCommandsHandler>(TAggregate agr = null, TCommandsHandler handler = null)
            where TAggregate : class, IAggregate
            where TCommandsHandler : class, IAggregateCommandsHandler<TAggregate>
        {
            return new Scenario<TAggregate, TCommandsHandler>(agr, handler);
        }
    }
    public class Scenario<TAggregate,TCommandsHandler>
        where TAggregate : class, IAggregate
        where TCommandsHandler : class, IAggregateCommandsHandler<TAggregate>
    {
        protected TCommandsHandler CommandsHandler { get; }
        public TAggregate Aggregate { get; private set; }
        public TAggregate GivenAggregate { get; private set; }

        protected DomainEvent[] ExpectedEvents { get; private set; }
        protected DomainEvent[] ProducedEvents { get; private set; }
        protected DomainEvent[] GivenEvents { get; private set; }
        protected Command[]   GivenCommands { get; private set; }

        private void AddEventInfo(string message, IEnumerable<DomainEvent> ev, StringBuilder builder)
        {
            builder.AppendLine();
            builder.AppendLine(message);
            builder.AppendLine();
            foreach (var e in ev)
            {
                builder.AppendLine($"Event:{e.GetType().Name} : ");
                builder.AppendLine(e.ToPropsString());
            }
            builder.AppendLine();
        }
        protected string CollectDebugInfo()
        {
            var sb = new StringBuilder();
            foreach (var cmd in GivenCommands)
                sb.AppendLine($"Command: {cmd.ToPropsString()}");

            AddEventInfo("Given events", GivenEvents, sb);
            AddEventInfo("Produced events", ProducedEvents, sb);
            AddEventInfo("Expected events", ExpectedEvents, sb);

            return sb.ToString();
        }
        
        internal Scenario(TAggregate agr = null, TCommandsHandler handler = null)
        {
            CommandsHandler = handler ?? CreateCommandsHandler();
            Aggregate = agr ?? CreateAggregate();
            GivenAggregate = agr ?? CreateAggregate();
        }

        public Scenario<TAggregate, TCommandsHandler> Given(params DomainEvent[] events)
        {
            GivenEvents = events;
            Aggregate.ApplyEvents(events);
            Aggregate.ClearUncommittedEvents();

            GivenAggregate.ApplyEvents(events);
            GivenAggregate.ClearUncommittedEvents();

            return this;
        }
        public Scenario<TAggregate, TCommandsHandler> Given(IEnumerable<DomainEvent> events)
        {
           return Given(events.ToArray());
        }

        public T GivenEvent<T>(Predicate<T> filter = null) where T : DomainEvent
        {
            var events = GivenEvents.OfType<T>();
            if (filter != null)
                events = events.Where(e => filter(e));
            return events.FirstOrDefault();
        }

        public T GivenCommand<T>(Predicate<T> filter = null) where T : ICommand
        {
            var commands = GivenCommands.OfType<T>();
            if (filter != null)
                commands = commands.Where(e => filter(e));
            return commands.FirstOrDefault();
        }

        public Scenario<TAggregate, TCommandsHandler> When(params Command[] commands)
        {
            GivenCommands = commands;
            return this;
        }

        public Scenario<TAggregate, TCommandsHandler> Run()
        {
            //When
            foreach (var cmd in GivenCommands)
                Aggregate = CommandsHandler.Execute(Aggregate, cmd);

            //Then
            ProducedEvents = Aggregate.GetUncommittedEvents()
                                      .Cast<DomainEvent>()
                                      .ToArray();

            return this;
          
        }

        public void Check()
        {
            Console.WriteLine(CollectDebugInfo());
            EventsExtensions.CompareEvents(ExpectedEvents, ProducedEvents);
        }

        public Scenario<TAggregate, TCommandsHandler> Then(IEnumerable<DomainEvent> expectedEvents)
        {
            return Then(expectedEvents.ToArray());
        }

        public Scenario<TAggregate, TCommandsHandler> Then(params DomainEvent[] expectedEvents)
        {
            ExpectedEvents = expectedEvents;
            return this;
        }

        private static TAggregate CreateAggregate()
        {
            return (TAggregate) (new AggregateFactory().Build(typeof(TAggregate), Guid.NewGuid(), null));
        }

        public static Scenario<TAggregate,TCommandsHandler> New(TAggregate agr = null, TCommandsHandler handler = null)
        {
            return new Scenario<TAggregate,TCommandsHandler>(agr,handler);
        }

        private static TCommandsHandler CreateCommandsHandler()
        {
            var constructorInfo = typeof(TCommandsHandler).GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
                throw new CannotCreateCommandHandlerExeption();

            return (TCommandsHandler)constructorInfo.Invoke(null);
        }

    }
}