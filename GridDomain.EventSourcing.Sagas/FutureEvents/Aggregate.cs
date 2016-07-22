using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonDomain.Core;
using GridDomain.Common;

namespace GridDomain.EventSourcing.Sagas.FutureEvents
{
    public class Aggregate : AggregateBase
    {

        #region AsyncMethods
        //keep track of all invocation to be sure only aggregate-initialized async events can be applied
        private readonly IDictionary<Guid, AsyncEventsInProgress> _asyncEventsResults = new Dictionary<Guid, AsyncEventsInProgress>();
        public readonly HashSet<AsyncEventsInProgress> AsyncUncomittedEvents = new HashSet<AsyncEventsInProgress>();

        public void RaiseEventAsync<TTask>(Task<TTask> eventProducer) where TTask : DomainEvent
        {
            var entityToArrayTask = eventProducer.ContinueWithSafeResultCast(@event => new DomainEvent[] { @event });
            RaiseEventAsync(entityToArrayTask);
        }

        protected void RaiseEventAsync(Task<DomainEvent[]> eventProducer)
        {
            var asyncMethodStarted = new AsyncEventsInProgress(eventProducer, Guid.NewGuid());
            AsyncUncomittedEvents.Add(asyncMethodStarted);
            _asyncEventsResults.Add(asyncMethodStarted.InvocationId, asyncMethodStarted);
        }

        public void FinishAsyncExecution(Guid invocationId)
        {
            AsyncEventsInProgress eventsInProgress;

            if (!_asyncEventsResults.TryGetValue(invocationId, out eventsInProgress)) return;
            if (!eventsInProgress.ResultProducer.IsCompleted)
                throw new NotFinishedAsyncMethodResultsRequestedException();
            _asyncEventsResults.Remove(invocationId);

            foreach (var @event in eventsInProgress.ResultProducer.Result)
                RaiseEvent(@event);
        }

        #endregion
        #region FutureEvents
        protected Aggregate(Guid id)
        {
            Id = id;
            Register<FutureEventScheduledEvent>(Apply);
            Register<FutureEventOccuredEvent>(Apply);
            Register<FutureEventCanceledEvent>(Apply);
        }

        private readonly IDictionary<Guid, FutureEventScheduledEvent> _futureEvents = new Dictionary<Guid, FutureEventScheduledEvent>();

        public void RaiseScheduledEvent(Guid futureEventId)
        {
            FutureEventScheduledEvent e;
            if (!_futureEvents.TryGetValue(futureEventId, out e))
                throw new ScheduledEventNotFoundException(futureEventId);

            RaiseEvent(new FutureEventOccuredEvent(Guid.NewGuid(), futureEventId, Id));
            RaiseEvent(e.Event);
        }

        protected void RaiseEvent(DateTime raiseTime, DomainEvent @event)
        {
            RaiseEvent(new FutureEventScheduledEvent(Guid.NewGuid(), Id, raiseTime, @event));
        }

        protected void CancelScheduledEvents<TEvent>(Predicate<TEvent> criteia) where TEvent : DomainEvent
        {
            var eventsToCancel = this._futureEvents.Values.Where(fe => (fe.Event is TEvent) && criteia((TEvent)fe.Event)).ToArray();

            var cancelEvents = eventsToCancel.Select(e => new FutureEventCanceledEvent(e.Id, Id));
            foreach (var e in cancelEvents)
                RaiseEvent(e);
        }

        private void Apply(FutureEventScheduledEvent e)
        {
            _futureEvents[e.Id] = e;
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
            if (!_futureEvents.TryGetValue(futureEventId, out evt)) return;
            _futureEvents.Remove(futureEventId);
        }

        #endregion

    }
}