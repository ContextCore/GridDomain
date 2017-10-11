using System.Collections.Generic;
using System.Linq;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.Tests.Common
{
    public static class AggregateDebugExtensions
    {
        public static void ApplyEvents(this Aggregate aggregate, params DomainEvent[] events)
        {
            foreach (var e in events)
            {
                ApplyEvent(aggregate,e);
            }
            
        }

        public static T ApplyEvent<T>(this Aggregate aggregate, T evt) where T:DomainEvent
        {
            ((IAggregate)aggregate).ApplyEvent(evt);
            return evt;
        }

        public static IReadOnlyCollection<TEvent> GetEvents<TEvent>(this IAggregate aggregate) where TEvent : DomainEvent
        {
            return aggregate.GetUncommittedEvents().OfType<TEvent>().ToArray();
        }

        public static IReadOnlyCollection<object> GetEvents(this IAggregate aggregate)
        {
            return GetEvents<DomainEvent>(aggregate);
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