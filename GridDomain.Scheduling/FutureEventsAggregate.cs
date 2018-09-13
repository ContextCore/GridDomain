using System;
using System.Collections.Generic;
using System.Linq;
using GridDomain.EventSourcing;

namespace GridDomain.Scheduling
{
    public class FutureEventsAggregate : ConventionAggregate
    {
        protected FutureEventsAggregate(string id):base(id)
        {
            _schedulingSourceName = GetType().Name;
            Execute<RaiseScheduledDomainEventCommand>(c => RaiseScheduledEvent(c.FutureEventId,c.AggregateId));
        }

        public IEnumerable<FutureEventScheduledEvent> FutureEvents  =>_futureEvents;
        readonly List<FutureEventScheduledEvent> _futureEvents = new List<FutureEventScheduledEvent>();
        private readonly string _schedulingSourceName;

      
        public void RaiseScheduledEvent(string futureEventId, string futureEventOccuredEventId)
        {
            FutureEventScheduledEvent ev = FutureEvents.FirstOrDefault(e => e.Id == futureEventId);
            if (ev == null)
                throw new ScheduledEventNotFoundException(futureEventId);

            var futureEventOccuredEvent = new FutureEventOccuredEvent(futureEventOccuredEventId, futureEventId, Id);

            Emit(ev.Event);
            Emit(futureEventOccuredEvent);
        }

        protected void Emit(DomainEvent @event, DateTime raiseTime, string futureEventId = null)
        {
             Emit(new FutureEventScheduledEvent(futureEventId ?? Guid.NewGuid().ToString(), Id, raiseTime, @event, _schedulingSourceName));
        }

        protected void CancelScheduledEvents<TEvent>(Predicate<TEvent> criteia = null) where TEvent : DomainEvent
        {
            var eventsToCancel = FutureEvents.Where(fe => fe.Event is TEvent);
            if (criteia != null)
                eventsToCancel = eventsToCancel.Where(e => criteia((TEvent) e.Event));

            var domainEvents = eventsToCancel.Select(e => new FutureEventCanceledEvent(e.Id, Id, _schedulingSourceName))
                                             .Cast<DomainEvent>()
                                             .ToArray();
            Emit(domainEvents);
        }

        protected void Apply(FutureEventScheduledEvent e)
        {
            _futureEvents.Add(e);
        }

        protected void Apply(FutureEventOccuredEvent e)
        {
            DeleteFutureEvent(e.FutureEventId);
        }

        protected void Apply(FutureEventCanceledEvent e)
        {
            DeleteFutureEvent(e.FutureEventId);
        }

        private void DeleteFutureEvent(string futureEventId)
        {
            FutureEventScheduledEvent evt = FutureEvents.FirstOrDefault(e => e.Id == futureEventId);
            if (evt == null)
                throw new ScheduledEventNotFoundException(futureEventId);
            _futureEvents.Remove(evt);
        }
    }
}