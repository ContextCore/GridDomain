using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GridDomain.EventSourcing;
using GridDomain.EventSourcing.CommonDomain;
using GridDomain.Scheduling.FutureEvents;

namespace GridDomain.Scheduling
{
    public class FutureEventsAggregate : Aggregate
    {
       

        protected FutureEventsAggregate(Guid id):base(id)
        {
            Register<FutureEventScheduledEvent>(Apply);
            Register<FutureEventOccuredEvent>(Apply);
            Register<FutureEventCanceledEvent>(Apply);
            _schedulingSourceName = GetType().Name;
        }

        #region Base functions

    
       
        // Aggregate State, do not mix with uncommited events 
        public IEnumerable<FutureEventScheduledEvent> FutureEvents  =>_futureEvents;
        readonly List<FutureEventScheduledEvent> _futureEvents = new List<FutureEventScheduledEvent>();
        private readonly string _schedulingSourceName;

        /// <summary>
        /// will emit occured event only after succesfull apply of scheduled event
        /// </summary>
        /// <param name="futureEventId"></param>
        /// <param name="futureEventOccuredEventId"></param>
        public void RaiseScheduledEvent(Guid futureEventId, Guid futureEventOccuredEventId)
        {
            FutureEventScheduledEvent ev = FutureEvents.FirstOrDefault(e => e.Id == futureEventId);
            if (ev == null)
                throw new ScheduledEventNotFoundException(futureEventId);

            var futureEventOccuredEvent = new FutureEventOccuredEvent(futureEventOccuredEventId, futureEventId, Id);

            //How to handle case when applying occured event will raise an exception?
             Emit(ev.Event,
                  futureEventOccuredEvent);
        }

        protected void Emit(DomainEvent @event, DateTime raiseTime, Guid? futureEventId = null)
        {
             Emit(new FutureEventScheduledEvent(futureEventId ?? Guid.NewGuid(), Id, raiseTime, @event, _schedulingSourceName));
        }

        protected void Emit(DomainEvent @event, Func<Task> afterApply, DateTime raiseTime, Guid? futureEventId = null)
        {
            Emit(new FutureEventScheduledEvent(futureEventId ?? Guid.NewGuid(), Id, raiseTime, @event, _schedulingSourceName));
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

        private void Apply(FutureEventScheduledEvent e)
        {
            _futureEvents.Add(e);
        }

        private void Apply(FutureEventOccuredEvent e)
        {
            DeleteFutureEvent(e.FutureEventId);
        }

        private void Apply(FutureEventCanceledEvent e)
        {
            DeleteFutureEvent(e.FutureEventId);
        }

        private void DeleteFutureEvent(Guid futureEventId)
        {
            FutureEventScheduledEvent evt = FutureEvents.FirstOrDefault(e => e.Id == futureEventId);
            if (evt == null)
                return;
            _futureEvents.Remove(evt);
        }
        
        #endregion
    }
}