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

        // Only for simple implementation 
       

        public IDictionary<Guid, FutureEventScheduledEvent> FutureEvents { get; } = new ConcurrentDictionary<Guid, FutureEventScheduledEvent>();

        /// <summary>
        /// will emit occured event only after succesfull apply of scheduled event
        /// </summary>
        /// <param name="futureEventId"></param>
        /// <param name="futureEventOccuredEventId"></param>
        /// <param name="afterEventsPersistence"></param>
        public void RaiseScheduledEvent(Guid futureEventId, Guid futureEventOccuredEventId, Action afterEventsPersistence = null)
        {
            FutureEventScheduledEvent e;
            if (!FutureEvents.TryGetValue(futureEventId, out e))
                throw new ScheduledEventNotFoundException(futureEventId);

            var futureEventOccuredEvent = new FutureEventOccuredEvent(futureEventOccuredEventId, futureEventId, Id);

            //will emit occured event only after succesfull apply of scheduled event
             Emit(e.Event, () => Emit(futureEventOccuredEvent, afterEventsPersistence));
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
            var eventsToCancel = FutureEvents.Values.Where(fe => fe.Event is TEvent);
            if (criteia != null)
                eventsToCancel = eventsToCancel.Where(e => criteia((TEvent) e.Event));

            var domainEvents = eventsToCancel.Select(e => new FutureEventCanceledEvent(e.Id, Id))
                                             .Cast<DomainEvent>()
                                             .ToArray();
            Emit(domainEvents);
        }

        private void Apply(FutureEventScheduledEvent e)
        {
            FutureEvents[e.Id] = e;
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
            FutureEventScheduledEvent evt;
            if (!FutureEvents.TryGetValue(futureEventId, out evt))
                return;
            FutureEvents.Remove(futureEventId);
        }

        #endregion
    }
}