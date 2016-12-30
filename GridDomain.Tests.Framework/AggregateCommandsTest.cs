using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommonDomain;
using GridDomain.CQRS;
using GridDomain.CQRS.Messaging.MessageRouting;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using NUnit.Framework;

namespace GridDomain.Tests.Framework
{
    public class AggregateCommandsTest<TAggregate, THandler>: AggregateTest<TAggregate>
        where TAggregate : IAggregate
        where THandler: class, IAggregateCommandsHandler<TAggregate>
    {
        protected THandler CommandsHandler { get; private set; }

        protected virtual THandler CreateCommandsHandler()
        {
            var constructorInfo = typeof(THandler).GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
                throw new CannotCreateCommandHandlerExcpetion();

            return (THandler) constructorInfo.Invoke(null);
        }

        protected DomainEvent[] ExecuteCommand(ICommand command)
        {
            CommandsHandler = CommandsHandler ?? CreateCommandsHandler();

            Aggregate = CommandsHandler.Execute(Aggregate, command);
            return ProducedEvents = Aggregate.GetUncommittedEvents()
                                             .Cast<DomainEvent>()
                                             .ToArray();
        }

        protected void Execute(ICommand command)
        {
            ExpectedEvents = Expected().ToArray();
            var events = ExecuteCommand(command);
            Console.WriteLine(CollectDebugInfo(command));
            EventsExtensions.CompareEvents(ExpectedEvents,events);
        }

        protected DomainEvent[] ExpectedEvents { get; private set; }
        protected DomainEvent[] ProducedEvents { get; private set; }
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
        protected string CollectDebugInfo(ICommand command)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Command: {command.ToPropsString()}");

            AddEventInfo("Given events",    GivenEvents, sb);
            AddEventInfo("Produced events", ProducedEvents, sb);
            AddEventInfo("Expected events", ExpectedEvents, sb);

            return sb.ToString();
        }

        protected virtual IEnumerable<DomainEvent> Expected()
        {
            return Enumerable.Empty<DomainEvent>();
        }
    }

    public class CannotCreateCommandHandlerExcpetion : Exception
    {
    }
}