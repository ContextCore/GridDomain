using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonDomain.Core;
using GridDomain.EventSourcing;

namespace GridDomain.Node.FutureEvents
{
    public class Aggregate : AggregateBase
    {
        private readonly IDictionary<Guid,FutureDomainEvent> _futureEvents;

        private readonly List<Task<DomainEvent>> _asyncEvents = new List<Task<DomainEvent>>();
        public ICollection<Task<DomainEvent>> AsyncEvents => _asyncEvents;

        protected void RaiseEvent(Task<DomainEvent> eventProducer)
        {
            _asyncEvents.Add(eventProducer.ContinueWith(t => 
            {
                RaiseEvent(t.Result);
                return t.Result;
            }));
        }

        protected Aggregate(Guid id)
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