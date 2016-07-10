using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonDomain.Core;
using GridDomain.EventSourcing;
using GridDomain.Node.Actors;

namespace GridDomain.Node.FutureEvents
{
    public class Aggregate : AggregateBase
    {
        protected void RaiseEvent(params DomainEvent[] events)
        {
            foreach(var ev in events)
                base.RaiseEvent(ev);
        }

        #region AsyncMethods

        public readonly List<AsyncMethodStarted> AsyncMethodsStarted = new List<AsyncMethodStarted>();

        protected void RaiseEvent(Task<DomainEvent[]> eventProducer)
        {
            var domainEventApplyToAggregateTask = 
            eventProducer.ContinueWith(t => 
            {
                RaiseEvent(t.Result);
                return t.Result;
            });
            AsyncMethodsStarted.Add(new AsyncMethodStarted(domainEventApplyToAggregateTask));
        }
        #endregion
        #region FutureEvents
        protected Aggregate(Guid id)
        {
            Id = id;
            Register<FutureDomainEvent>(Apply);
        }
        private readonly IDictionary<Guid, FutureDomainEvent> _futureEvents = new Dictionary<Guid, FutureDomainEvent>();

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
        #endregion
    }
}