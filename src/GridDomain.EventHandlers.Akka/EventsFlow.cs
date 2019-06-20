using Akka;
using Akka.Persistence.Query;
using Akka.Streams.Dsl;
using GridDomain.Aggregates.Abstractions;

namespace GridDomain.EventHandlers.Akka
{
    public static class EventsFlow
    {
        public static Flow<EventEnvelope, TEvent, NotUsed> Create<TEvent>() where TEvent: class, IDomainEvent
        {
            return Flow.Create<EventEnvelope>()
                .Select(e => e.Event as TEvent);
        }
    }
}