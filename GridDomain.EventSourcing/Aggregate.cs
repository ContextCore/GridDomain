using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonDomain;
using CommonDomain.Core;
using GridDomain.EventSourcing.FutureEvents;

namespace GridDomain.EventSourcing
{
    public class Aggregate : AggregateBase,
                             IMemento
    {
        private static readonly AggregateFactory Factory = new AggregateFactory();

        // Only for simple implementation 
        Guid IMemento.Id
        {
            get { return Id; }
            set { Id = value; }
        }

        int IMemento.Version
        {
            get { return Version; }
            set { Version = value; }
        }

        public static T Empty<T>(Guid id) where T : IAggregate
        {
            return Factory.Build<T>(id);
        }

        protected void Apply<T>(Action<T> action) where T : DomainEvent
        {
            Register(action);
        }

        protected override IMemento GetSnapshot()
        {
            return this;
        }

        #region AsyncMethods

        //keep track of all invocation to be sure only aggregate-initialized async events can be applied
        private readonly IDictionary<Guid, AsyncEventsInProgress> _asyncEventsResults =
            new Dictionary<Guid, AsyncEventsInProgress>();

        private readonly HashSet<AsyncEventsInProgress> _asyncUncomittedEvents = new HashSet<AsyncEventsInProgress>();

        public void ClearAsyncUncomittedEvents()
        {
            _asyncUncomittedEvents.Clear();
        }

        public IReadOnlyCollection<AsyncEventsInProgress> GetAsyncUncomittedEvents()
        {
            return _asyncUncomittedEvents;
        }

        public IDictionary<Guid, FutureEventScheduledEvent> FutureEvents { get; } =
            new Dictionary<Guid, FutureEventScheduledEvent>();

        public void RaiseEventAsync<TTask>(Task<TTask> eventProducer) where TTask : DomainEvent
        {
            var entityToArrayTask = eventProducer.ContinueWith(t => new DomainEvent[] {t.Result});
            RaiseEventAsync(entityToArrayTask);
        }

        protected void RaiseEventAsync(Task<DomainEvent[]> eventProducer)
        {
            var asyncMethodStarted = new AsyncEventsInProgress(eventProducer, Guid.NewGuid());
            _asyncUncomittedEvents.Add(asyncMethodStarted);
            _asyncEventsResults.Add(asyncMethodStarted.InvocationId, asyncMethodStarted);
        }

        public void FinishAsyncExecution(Guid invocationId)
        {
            AsyncEventsInProgress eventsInProgress;

            if (!_asyncEventsResults.TryGetValue(invocationId, out eventsInProgress)) return;
            if (!eventsInProgress.ResultProducer.IsCompleted) throw new NotFinishedAsyncMethodResultsRequestedException();
            _asyncEventsResults.Remove(invocationId);

            foreach (var @event in eventsInProgress.ResultProducer.Result) RaiseEvent(@event);
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

        public void RaiseScheduledEvent(Guid futureEventId, Guid futureEventOccuredEventId)
        {
            FutureEventScheduledEvent e;
            if (!FutureEvents.TryGetValue(futureEventId, out e)) throw new ScheduledEventNotFoundException(futureEventId);

            RaiseEvent(e.Event);
            RaiseEvent(new FutureEventOccuredEvent(futureEventOccuredEventId, futureEventId, Id));
        }

        protected void RaiseEvent(DomainEvent @event, DateTime raiseTime, Guid? futureEventId = null)
        {
            RaiseEvent(new FutureEventScheduledEvent(futureEventId ?? Guid.NewGuid(), Id, raiseTime, @event));
        }

        protected void CancelScheduledEvents<TEvent>(Predicate<TEvent> criteia = null) where TEvent : DomainEvent
        {
            var eventsToCancel = FutureEvents.Values.Where(fe => fe.Event is TEvent);
            if (criteia != null) eventsToCancel = eventsToCancel.Where(e => criteia((TEvent) e.Event));

            foreach (var e in eventsToCancel.Select(e => new FutureEventCanceledEvent(e.Id, Id)).ToArray()) RaiseEvent(e);
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
            if (!FutureEvents.TryGetValue(futureEventId, out evt)) return;
            FutureEvents.Remove(futureEventId);
        }

        #endregion
    }
}