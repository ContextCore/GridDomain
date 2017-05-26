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
    public class Aggregate : AggregateBase,
                             IMemento,
                             IAggregate
    {
        private static readonly AggregateFactory Factory = new AggregateFactory();
        private static readonly Action EmptyContinue = () => { };
        private readonly IDictionary<Guid, DomainEvent> _eventToPersist = new ConcurrentDictionary<Guid, DomainEvent>();
        private int _emmitingMethodsInProgressCount;
        private Action<Task<Aggregate>, Action> _persistEvents = (newStateTask, afterAll) => { };
        public bool IsMethodExecuting => _emmitingMethodsInProgressCount > 0;
        public bool HasUncommitedEvents => _eventToPersist.Any();

        protected Aggregate(Guid id)
        {
            Id = id;
            RegisterPersistence((newStateTask, afterAll) =>
                                {
                                    newStateTask.Wait();
                                    afterAll();
                                });

            Register<FutureEventScheduledEvent>(Apply);
            Register<FutureEventOccuredEvent>(Apply);
            Register<FutureEventCanceledEvent>(Apply);
        }

        #region Base functions

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

        ICollection IAggregate.GetUncommittedEvents()
        {
            return (ICollection) _eventToPersist.Values;
        }

        void IAggregate.ClearUncommittedEvents()
        {
            _eventToPersist.Clear();
        }

        #endregion

        #region Persistence

        public bool MarkPersisted(DomainEvent e)
        {
            var evt = _eventToPersist[e.Id];
            RaiseEvent(evt);
            return _eventToPersist.Remove(e.Id);
        }

        public void RegisterPersistence(Action<Task<Aggregate>, Action> persistDelegate)
        {
            _persistEvents = persistDelegate;
        }

        #endregion

        #region Emitting events

        protected void Emit(params DomainEvent[] e)
        {
            Emit(EmptyContinue, e);
        }


        protected void Emit(DomainEvent @event, Action afterPersist)
        {
            Emit(afterPersist, @event);
        }

        protected void Emit(Action afterPersist, params DomainEvent[] events)
        {
            Emit(Task.FromResult(events), afterPersist);
        }

        protected void Emit<T>(Task<T> evtTask) where T : DomainEvent
        {
            Emit(evtTask.ContinueWith(t => new DomainEvent[] {t.Result}), EmptyContinue);
        }

        protected void Emit(Task<DomainEvent[]> evtTask, Action continuation = null)
        {
            Interlocked.Increment(ref _emmitingMethodsInProgressCount);
            var newStateTask = evtTask.ContinueWith(t =>
                                                    {
                                                        try
                                                        {
                                                            foreach (var e in t.Result)
                                                                _eventToPersist.Add(e.Id, e);
                                                        }
                                                        finally
                                                        {
                                                            Interlocked.Decrement(ref _emmitingMethodsInProgressCount);
                                                        }
                                                        
                                                        return this;
                                                    }, TaskContinuationOptions.AttachedToParent);

            _persistEvents(newStateTask, continuation ?? EmptyContinue);
        }

        #endregion

        #region FutureEvents

        public IDictionary<Guid, FutureEventScheduledEvent> FutureEvents { get; } = new ConcurrentDictionary<Guid, FutureEventScheduledEvent>();

        public void RaiseScheduledEvent(Guid futureEventId, Guid futureEventOccuredEventId)
        {
            FutureEventScheduledEvent e;
            if (!FutureEvents.TryGetValue(futureEventId, out e))
                throw new ScheduledEventNotFoundException(futureEventId);

            var futureEventOccuredEvent = new FutureEventOccuredEvent(futureEventOccuredEventId, futureEventId, Id);

            //will emit occured event only after succesfull apply & persist of scheduled event
            Emit(e.Event, () => Emit(futureEventOccuredEvent));
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