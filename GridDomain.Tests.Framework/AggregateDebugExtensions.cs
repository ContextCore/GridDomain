using System.Linq;
using CommonDomain;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Framework
{
    public static class AggregateDebugExtensions
    {

        public static void ApplyEvents(this IAggregate aggregate, params DomainEvent[] events)
        {
            foreach(var e in events)
                aggregate.ApplyEvent(e);
        }

        public static void ClearEvents(this IAggregate aggregate)
        {
            aggregate.ClearUncommittedEvents();
        }

        public static TEvent[] GetEvents<TEvent>(this IAggregate aggregate) where TEvent:DomainEvent
        {
            return aggregate.GetUncommittedEvents().OfType<TEvent>().ToArray();
        }

        public static TEvent GetEvent<TEvent>(this IAggregate aggregate) where TEvent : DomainEvent
        {
            var @event = aggregate.GetUncommittedEvents().OfType<TEvent>().FirstOrDefault();
            if (@event == null)
                throw new CannotFindRequestedEventException();
            return @event;
        }
    }
}