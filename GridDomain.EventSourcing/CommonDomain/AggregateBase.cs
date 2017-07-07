using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GridDomain.EventSourcing.CommonDomain
{
    public abstract class AggregateBase : IAggregate,
                                          IMemento,
                                          IEquatable<IAggregate>
    {
        protected internal readonly ICollection<object> _uncommittedEvents = new LinkedList<object>();
        public bool HasUncommitedEvents => _uncommittedEvents.Any();
        private IRouteEvents _registeredRoutes;

        private static readonly Func<Task> EmptyContinue = () => Task.CompletedTask;
        private Action<Func<Task>> _persistEvents = afterAll => afterAll();

        protected AggregateBase()
            : this(null) { }

        protected AggregateBase(IRouteEvents handler)
        {
            if (handler == null)
                return;

            RegisteredRoutes = handler;
            RegisteredRoutes.Register(this);
        }

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

        protected void Apply<T>(Action<T> action) where T : DomainEvent
        {
            Register(action);
        }

        public virtual IMemento GetSnapshot()
        {
            return this;
        }

        protected IRouteEvents RegisteredRoutes
        {
            get { return _registeredRoutes ?? (_registeredRoutes = new ConventionEventRouter(true, this)); }
            set
            {
                if (value == null)
                    throw new InvalidOperationException("AggregateBase must have an event router to function");

                _registeredRoutes = value;
            }
        }

        public Guid Id { get; protected set; }
        public int Version { get; protected set; }

        void IAggregate.ApplyEvent(object @event)
        {
            RegisteredRoutes.Dispatch(@event);
            Version++;
        }

        ICollection IAggregate.GetUncommittedEvents()
        {
            return (ICollection) _uncommittedEvents;
        }

        void IAggregate.ClearUncommittedEvents()
        {
            _uncommittedEvents.Clear();
        }

        public virtual bool Equals(IAggregate other)
        {
            return null != other && other.Id == Id;
        }

        protected void Register<T>(Action<T> route)
        {
            RegisteredRoutes.Register(route);
        }

        public void RegisterPersistence(Action<Func<Task>> saveStateAction)
        {
            _persistEvents = saveStateAction;
        }

        protected void Emit(params DomainEvent[] e)
        {
            Emit(EmptyContinue, e);
        }

        protected void Emit(DomainEvent @event, Action afterPersist)
        {
            Emit(() =>
                 {
                     afterPersist();
                     return Task.CompletedTask;
                 },
                 @event);
        }

        protected void Emit<T>(Task<T> evtTask) where T : DomainEvent
        {
            Emit(evtTask.ContinueWith(t => new DomainEvent[] {t.Result}), null);
        }

        public bool MarkPersisted(DomainEvent e)
        {
            if (!_uncommittedEvents.Contains(e))
                throw new EventIsNotBelongingToAggregateException();
            ((IAggregate) this).ApplyEvent(e);
            return _uncommittedEvents.Remove(e);
        }

        protected void Emit(Func<Task> afterPersist, params DomainEvent[] events)
        {
            foreach (var e in events)
            {
                _uncommittedEvents.Add(e);
            }

            _persistEvents(afterPersist);
        }

        /// <summary>
        /// returns task finishing when event will be procuded.
        /// No persistence is guaranted. 
        /// Use continuation task for run code after persistence.
        /// </summary>
        /// <param name="evtTask"></param>
        /// <param name="continuation"></param>
        /// <returns></returns>
        protected Task Emit(Task<DomainEvent[]> evtTask, Func<Task> continuation = null)
        {
            continuation = continuation ?? EmptyContinue;
            return evtTask.ContinueWith(t => Emit(continuation, t.Result));
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IAggregate);
        }
    }
}