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
    public class Scenario<TAggregate,TCommandsHandler>
        where TAggregate : class, IAggregate
        where TCommandsHandler : class, IAggregateCommandsHandler<TAggregate>
    {
        protected TCommandsHandler CommandsHandler { get; }
        protected TAggregate Aggregate { get; private set; }

        protected DomainEvent[] ExpectedEvents { get; private set; }
        protected DomainEvent[] ProducedEvents { get; private set; }
        protected DomainEvent[] GivenEvents { get; private set; }
        protected ICommand[]   GivenCommands { get; private set; }

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
        
        private Scenario(TAggregate agr = null, TCommandsHandler handler = null)
        {
            CommandsHandler = handler ?? CreateCommandsHandler();
            Aggregate = agr ?? CreateAggregate();
        }

        public Scenario<TAggregate, TCommandsHandler> Given(params DomainEvent[] events)
        {
            GivenEvents = events;
            Aggregate.ApplyEvents(events);
            return this;
        }

        public Scenario<TAggregate, TCommandsHandler> When(params Command[] commands)
        {
            GivenCommands = commands;
            foreach (var cmd in commands)
                Aggregate = CommandsHandler.Execute(Aggregate, cmd);

            return this;
        }

        public Scenario<TAggregate, TCommandsHandler> Then(params DomainEvent[] expectedEvents)
        {
            ExpectedEvents = expectedEvents;

            var produced = Aggregate.GetUncommittedEvents()
                                    .Cast<DomainEvent>()
                                    .ToArray();

            Console.WriteLine(CollectDebugInfo());
            EventsExtensions.CompareEvents(expectedEvents, produced);

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