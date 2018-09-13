using System;
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
            ((IAggregate)aggregate).Apply(evt);
            return evt;
        }

        public static IReadOnlyCollection<TEvent> GetEvents<TEvent>(this IEventList aggregate) where TEvent : DomainEvent
        {
            return aggregate.Events.OfType<TEvent>().ToArray();
        }

        public static IReadOnlyCollection<object> GetEvents(this IEventList aggregate)
        {
            return GetEvents<DomainEvent>(aggregate);
        }

        public static TEvent GetEvent<TEvent>(this IEventList aggregate) where TEvent : DomainEvent
        {
            var @event = aggregate.Events.OfType<TEvent>().FirstOrDefault();
            if (@event == null)
                throw new CannotFindRequestedEventException();
            return @event;
        }

    
    }
}