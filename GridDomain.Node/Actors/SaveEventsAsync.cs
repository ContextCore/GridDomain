using System;
using GridDomain.EventSourcing;

namespace GridDomain.Node.Actors
{
    class SaveEventsAsync
    {
        public SaveEventsAsync(DomainEvent[] events, Action<DomainEvent> onEventPersisted, Action continuation, Aggregate state)
        {
            Continuation = continuation;
            State = state;
            Events = events;
            OnEventPersisted = onEventPersisted;
        }

        public Action Continuation { get; }
        public DomainEvent[] Events { get; }
        public Aggregate State { get; }
        public Action<DomainEvent> OnEventPersisted { get; }
    }
}