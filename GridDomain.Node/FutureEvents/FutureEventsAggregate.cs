using System;
using System.Collections.Generic;
using CommonDomain.Core;

namespace GridDomain.Node.FutureEvents
{
    public class FutureEventsAggregate : AggregateBase
    {
        private readonly IDictionary<Guid,FutureDomainEvent> _futureEvents;

        protected FutureEventsAggregate(Guid id)
        {
            _futureEvents = new Dictionary<Guid, FutureDomainEvent>();
            Id = id;
            Register<FutureDomainEvent>(Apply);
        }

        public void RaiseScheduledEvent(Guid eventId)
        {
            FutureDomainEvent e;
            if (!_futureEvents.TryGetValue(eventId, out e)) return;
            RaiseEvent(e.Event);
        }

        private void Apply(FutureDomainEvent e)
        {
            _futureEvents.Add(e.SourceId,e);
        }

   
    }
}