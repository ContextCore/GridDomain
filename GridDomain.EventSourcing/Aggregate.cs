using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GridDomain.CQRS;
using GridDomain.EventSourcing.CommonDomain;

namespace GridDomain.EventSourcing
{

    public class Aggregate : IAggregate,
                             IMemento,
                             IEquatable<IAggregate>
    {
        private static readonly AggregateFactory Factory = new AggregateFactory();
        public static T Empty<T>(Guid? id = null) where T : IAggregate
        {
            return Factory.Build<T>(id ?? Guid.NewGuid());
        }

        private readonly List<DomainEvent> _uncommittedEvents = new List<DomainEvent>(7);

        public void ClearUncommitedEvents()
        {
            _uncommittedEvents.Clear();
        }
        private PersistenceDelegate _persist;

        public bool HasUncommitedEvents => _uncommittedEvents.Any();

        public void SetPersistProvider(PersistenceDelegate caller)
        {
            _persist = caller;
        }

        protected Aggregate(Guid id)
        {
            Id = id;
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

        public virtual IMemento GetSnapshot()
        {
            return this;
        }
        //TODO: think how to reduce pain from static cache 

        protected IRouteEvents _registeredRoutes;
        private IRouteEvents RegisteredRoutes => _registeredRoutes ?? (_registeredRoutes = EventRouterCache.Instance.Get(this.GetType()));
        //private IRouteCommands RegisteredCommands 

        public Guid Id { get; protected set; }
        public int Version { get; protected set; }

        void IAggregate.ApplyEvent(DomainEvent @event)
        {
            RegisteredRoutes.Dispatch(this,@event);
            Version++;
        }

        IReadOnlyCollection<DomainEvent> IAggregate.GetUncommittedEvents()
        {
            return  _uncommittedEvents;
        }

        public virtual bool Equals(IAggregate other)
        {
            return null != other && other.Id == Id;
        }

        protected async Task Emit<T>(Task<T> evtTask) where T : DomainEvent
        {
            await Emit(await evtTask);
        }

        public bool MarkPersisted(DomainEvent e)
        {
            if (!_uncommittedEvents.Contains(e))
                throw new EventIsNotBelongingToAggregateException();

            ((IAggregate) this).ApplyEvent(e);
            return _uncommittedEvents.Remove(e);
        }

        protected async Task Emit(params DomainEvent[] events)
        {
            Produce(events);
            await _persist(this);
        }

        protected void Produce(params DomainEvent[] events)
        {
            foreach(var e in events)
            {
                _uncommittedEvents.Add(e);
            }
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