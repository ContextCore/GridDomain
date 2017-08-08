using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors.Aggregates.Messages
{
    class PersistEventPack
    {
        public DomainEvent[] Events { get; }

        internal PersistEventPack(params DomainEvent[] events )
        {
            Events = events;
        }
    }
}