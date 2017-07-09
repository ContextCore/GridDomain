using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.Aggregates.Messages
{
    class PersistEvents
    {
        public DomainEvent[] Events { get; }

        internal PersistEvents(params DomainEvent[] events )
        {
            Events = events;
        }
    }

    class EventsPersisted
    {

        private EventsPersisted()
        {
        }

        public static EventsPersisted Instance { get; } = new EventsPersisted();
    }
}