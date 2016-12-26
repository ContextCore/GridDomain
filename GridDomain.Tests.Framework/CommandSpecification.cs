using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GridDomain.CQRS;
using GridDomain.EventSourcing;
using GridDomain.Logging;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace GridDomain.Tests.Framework
{
    public abstract class CommandSpecification<TCommand> where TCommand : class, CQRS.ICommand
    {
        protected DomainEvent[] ExpectedEvents;

        protected DomainEvent[] GivenEvents;
        protected TCommand Command { get; private set; }
        protected InMemoryEventRepository Repository { get; private set; }
        protected abstract ICommandHandler<TCommand> Handler { get; }

        //     protected IMessageTransport Bus { get; private set; }

        protected virtual TCommand CommandFactory()
        {
            return new Fixture().Create<TCommand>();
        }


        private void AddEventInfo(string message, IEnumerable<DomainEvent> ev, StringBuilder builder)
        {
            builder.AppendLine();
            builder.AppendLine(message);
            builder.AppendLine();
            foreach (var e in GivenEvents)
            {
                builder.AppendLine($"Event:{e.GetType().Name} : ");
                builder.AppendLine(e.ToPropsString());
            }
            builder.AppendLine();
        }

        protected string CollectDebugInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Command: {Command.ToPropsString()}");

            AddEventInfo("Given events", GivenEvents, sb);
            AddEventInfo("Produced events", Repository.ProducedEvents, sb);
            AddEventInfo("Expected events", ExpectedEvents, sb);
            return sb.ToString();
        }

        protected void PrintDebugInfo()
        {
            Console.WriteLine(CollectDebugInfo());
        }

        [SetUp]
        public void Init()
        {
            Command = CommandFactory();
            GivenEvents = Given().ToArray();
            ExpectedEvents = Expected().ToArray();
            Repository = CreateRepository();
        }

        protected List<DomainEvent> ExecuteCommand()
        {
            try
            {
                Handler.Handle(Command);
            }
            catch
            {
                PrintDebugInfo();
                throw;
            }
            return Repository.ProducedEvents;
        }

        protected void VerifyExpected()
        {
            EventsExtensions.CompareEvents(ExpectedEvents,
                ExecuteCommand());
        }

        protected virtual IEnumerable<DomainEvent> Expected()
        {
            return Enumerable.Empty<DomainEvent>();
        }

        protected virtual InMemoryEventRepository CreateRepository()
        {
            var repo = new InMemoryEventRepository(new AggregateFactory());
            foreach (var ev in GivenEvents)
                repo.AddEvent(ev);

            return repo;
        }

        protected virtual IEnumerable<DomainEvent> Given()
        {
            yield break;
        }
    }
}