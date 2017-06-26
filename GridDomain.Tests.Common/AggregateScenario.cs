using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using Ploeh.AutoFixture;

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
        public Guid Id { get; }= Guid.NewGuid();

        public AggregateScenario(TAggregate aggregate, IAggregateCommandsHandler<TAggregate> handler)
        {
            CommandsHandler = handler ?? throw new ArgumentNullException(nameof(handler));
            Aggregate = aggregate ?? throw new ArgumentNullException(nameof(aggregate));
        }


        private IAggregateCommandsHandler<TAggregate> CommandsHandler { get; }
        public TAggregate Aggregate { get; private set; }

        private DomainEvent[] ExpectedEvents { get; set; } = {};
        private DomainEvent[] ProducedEvents { get; set; } = {};
        private DomainEvent[] GivenEvents { get; set; } = {};
        private List<Command> GivenCommands { get; set; } = new List<Command>();

        private void AddEventInfo(string message, IEnumerable<DomainEvent> ev, StringBuilder builder)
        {
            builder.AppendLine();
            builder.AppendLine(message);
            builder.AppendLine();
            foreach (var e in ev)
            {
                builder.AppendLine($"Event:{e?.GetType().Name} : ");
                builder.AppendLine(e?.ToPropsString());
            }
            builder.AppendLine();
        }

        private string CollectDebugInfo()
        {
            var sb = new StringBuilder();
            foreach (var cmd in GivenCommands)
                sb.AppendLine($"Command: {cmd.ToPropsString()}");

            AddEventInfo("Given events", GivenEvents, sb);
            AddEventInfo("Produced events", ProducedEvents, sb);
            AddEventInfo("Expected events", ExpectedEvents, sb);

            return sb.ToString();
        }

        public AggregateScenario<TAggregate> Given(params DomainEvent[] events)
        {
            GivenEvents = events;
            Aggregate.ApplyEvents(events);
            return this;
        }

        public AggregateScenario<TAggregate> Given(IEnumerable<DomainEvent> events)
        {
            return Given(events.ToArray());
        }

        public AggregateScenario<TAggregate> When(params Command[] commands)
        {
            GivenCommands = commands.ToList();
            return this;
        }

        public AggregateScenario<TAggregate> Then(IEnumerable<DomainEvent> expectedEvents)
        {
            return Then(expectedEvents.ToArray());
        }

        public AggregateScenario<TAggregate> Then(params DomainEvent[] expectedEvents)
        {
            ExpectedEvents = expectedEvents;
            return this;
        }

        public AggregateScenario<TAggregate> Run()
        {
            RunAsync().Wait();
            return this;
        }
        public async Task RunAsync()
        {
            //When
            foreach (var cmd in GivenCommands)
                Aggregate = await CommandsHandler.ExecuteAsync(Aggregate, cmd);

            //Then
            ProducedEvents = Aggregate.GetDomainEvents();

            Aggregate.PersistAll();
        }

        public AggregateScenario<TAggregate> Check()
        {
            Console.WriteLine(CollectDebugInfo());
            EventsExtensions.CompareEvents(ExpectedEvents, ProducedEvents);
            return this;
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
    }
}