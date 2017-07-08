using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing
{
    public abstract class Aggregate : IAggregate,
                                          IMemento,
                                          IEquatable<IAggregate>
    {
        private static readonly AggregateFactory Factory = new AggregateFactory();
        public static T Empty<T>(Guid? id = null) where T : IAggregate
        {
            return Factory.Build<T>(id ?? Guid.NewGuid());
        }

        private readonly ICollection<object> _uncommittedEvents = new LinkedList<object>();
        public bool HasUncommitedEvents => _uncommittedEvents.Any();
        private IRouteEvents _registeredRoutes;
        public Func<DomainEvent[], Task> Persist { get; set; }

        protected Aggregate(Guid id) : this(null)
        {
            Id = id;
        }

        protected Aggregate(IRouteEvents handler)
        {
            if (handler == null)
                return;

            RegisteredRoutes = handler;
            RegisteredRoutes.Register(this);
        }

        Guid IMemento.Id
        {
            get => Id;
            set => Id = value;
        }

        int IMemento.Version
        {
            get => Version;
            set => Version = value;
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
            get => _registeredRoutes ?? (_registeredRoutes = new ConventionEventRouter(true, this));
            set => _registeredRoutes = value ?? throw new InvalidOperationException("AggregateBase must have an event router to function");
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

        protected async Task Emit<T>(Task<T> evtTask) where T : DomainEvent
        {
            Emit(await evtTask);
        }

        public bool MarkPersisted(DomainEvent e)
        {
            if (!_uncommittedEvents.Contains(e))
                throw new EventIsNotBelongingToAggregateException();
            ((IAggregate) this).ApplyEvent(e);
            return _uncommittedEvents.Remove(e);
        }

        protected Task Emit(params DomainEvent[] events)
        {
            foreach (var e in events)
            {
                _uncommittedEvents.Add(e);
            }
            return Persist(_uncommittedEvents.Cast<DomainEvent>().ToArray());
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