using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


using GridDomain.EventSourcing.CommonDomain;
using GridDomain.EventSourcing.FutureEvents;

namespace GridDomain.EventSourcing
{
    public class Aggregate : AggregateBase
    {
        private static readonly AggregateFactory Factory = new AggregateFactory();
       

        protected Aggregate(Guid id)
        {
            Id = id;
            Register<FutureEventScheduledEvent>(Apply);
            Register<FutureEventOccuredEvent>(Apply);
            Register<FutureEventCanceledEvent>(Apply);
        }

        #region Base functions

    
        public static T Empty<T>(Guid id) where T : IAggregate
        {
            return Factory.Build<T>(id);
        }

        // Aggregate State, do not mix with uncommited events 
        public IEnumerable<FutureEventScheduledEvent> FutureEvents  =>_futureEvents;//new List<FutureEventScheduledEvent>();
        readonly List<FutureEventScheduledEvent> _futureEvents = new List<FutureEventScheduledEvent>();
        /// <summary>
        /// will emit occured event only after succesfull apply of scheduled event
        /// </summary>
        /// <param name="futureEventId"></param>
        /// <param name="futureEventOccuredEventId"></param>
        /// <param name="afterEventsPersistence"></param>
        public void RaiseScheduledEvent(Guid futureEventId, Guid futureEventOccuredEventId, Action afterEventsPersistence = null)
        {
            FutureEventScheduledEvent ev = FutureEvents.FirstOrDefault(e => e.Id == futureEventId);
            if (ev == null)
                throw new ScheduledEventNotFoundException(futureEventId);

            var futureEventOccuredEvent = new FutureEventOccuredEvent(futureEventOccuredEventId, futureEventId, Id);

            //will emit occured event only after succesfull apply of scheduled event
             Emit(ev.Event, () => Emit(futureEventOccuredEvent, afterEventsPersistence));
        }

        protected void Emit(DomainEvent @event, DateTime raiseTime, Guid? futureEventId = null)
        {
             Emit(new FutureEventScheduledEvent(futureEventId ?? Guid.NewGuid(), Id, raiseTime, @event));
        }

        protected void Emit(DomainEvent @event, Action afterApply, DateTime raiseTime, Guid? futureEventId = null)
        {
            Emit(afterApply, new FutureEventScheduledEvent(futureEventId ?? Guid.NewGuid(), Id, raiseTime, @event));
        }

        protected void CancelScheduledEvents<TEvent>(Predicate<TEvent> criteia = null) where TEvent : DomainEvent
        {
            var eventsToCancel = FutureEvents.Where(fe => fe.Event is TEvent);
            if (criteia != null)
                eventsToCancel = eventsToCancel.Where(e => criteia((TEvent) e.Event));

            var domainEvents = eventsToCancel.Select(e => new FutureEventCanceledEvent(e.Id, Id))
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