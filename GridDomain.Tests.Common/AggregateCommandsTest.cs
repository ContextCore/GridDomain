using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Logging;

namespace GridDomain.Tests.Common
{
    public class AggregateCommandsTest<TAggregate, THandler> : AggregateTest<TAggregate> where TAggregate : Aggregate
                                                                                         where THandler : class,
                                                                                         IAggregateCommandsHandler
                                                                                         <TAggregate>
    {
        protected THandler CommandsHandler { get; private set; }

        protected DomainEvent[] ExpectedEvents { get; private set; }
        protected DomainEvent[] ProducedEvents { get; private set; }

        protected virtual THandler CreateCommandsHandler()
        {
            var constructorInfo = typeof(THandler).GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null)
                throw new CannotCreateCommandHandlerExeption();

            return (THandler) constructorInfo.Invoke(null);
        }

        protected async Task<DomainEvent[]> ExecuteCommand(params ICommand[] command)
        {
            CommandsHandler = CommandsHandler ?? CreateCommandsHandler();

            foreach (var cmd in command)
                Aggregate = await CommandsHandler.ExecuteAsync(Aggregate, cmd);

            ProducedEvents = Aggregate.GetDomainEvents();
            Aggregate.PersistAll();
            return ProducedEvents;
        }

        protected async Task Execute(params ICommand[] command)
        {
            ExpectedEvents = Expected().ToArray();
            var events = await ExecuteCommand(command);
            Console.WriteLine(CollectDebugInfo(command));
            EventsExtensions.CompareEvents(ExpectedEvents, events);
        }

        protected async Task RunScenario(IEnumerable<DomainEvent> given,
                                         IEnumerable<DomainEvent> expected,
                                         params ICommand[] command)
        {
            CommandsHandler = CommandsHandler ?? CreateCommandsHandler();

            Aggregate = (TAggregate) aggregateFactory.Build(typeof(TAggregate), Guid.NewGuid(), null);

            GivenEvents = given.ToArray();
            Aggregate.ApplyEvents(GivenEvents);

            foreach (var cmd in command)
                Aggregate = await CommandsHandler.ExecuteAsync(Aggregate, cmd);

            ProducedEvents = Aggregate.GetDomainEvents();
            Aggregate.PersistAll();

            Console.WriteLine(CollectDebugInfo(command));
            EventsExtensions.CompareEvents(expected.ToArray(), ProducedEvents);
        }

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

        protected string CollectDebugInfo(params ICommand[] commands)
        {
            var sb = new StringBuilder();
            foreach (var cmd in commands)
                sb.AppendLine($"Command: {cmd.ToPropsString()}");

            AddEventInfo("Given events", GivenEvents, sb);
            AddEventInfo("Produced events", ProducedEvents, sb);
            AddEventInfo("Expected events", ExpectedEvents, sb);

            return sb.ToString();
        }

        protected virtual IEnumerable<DomainEvent> Expected()
        {
            return Enumerable.Empty<DomainEvent>();
        }
    }
}