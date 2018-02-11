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
    public abstract class Aggregate : IAggregate,
                                      IMemento,
                                      IEquatable<IAggregate>
    {

        private readonly List<DomainEvent> _uncommittedEvents = new List<DomainEvent>(7);

        public void CommitAll()
        {
            foreach(var e in _uncommittedEvents)
                ((IAggregate)this).ApplyEvent(e);

            _uncommittedEvents.Clear();
        }
        protected IEventStore EventStore;

        public bool HasUncommitedEvents => _uncommittedEvents.Any();
       
        protected Aggregate(string id)
        {
            Id = id;
        }

        string IMemento.Id
        {
            get => Id;
            set => Id = value;
        }

        int IMemento.Version
        {
            get => Version;
            set => Version = value;
        }

        public string Id { get; protected set; }
        public int Version { get; protected set; }

        public void InitEventStore(IEventStore store)
        {
            EventStore = store;
        }

        void IAggregate.ApplyEvent(DomainEvent @event)
        {
            OnAppyEvent(@event);
            Version++;
        }

        protected abstract void OnAppyEvent(DomainEvent evt);

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

        public void Commit(DomainEvent e)
        {
            if (!_uncommittedEvents.Remove(e))
                throw new EventIsNotBelongingToAggregateException();

            ((IAggregate) this).ApplyEvent(e);
        }

       
        protected async Task Emit(params DomainEvent[] events)
        {
            Produce(events);
            await EventStore.Persist(this);
            CommitAll();
        }

        protected void Produce(params DomainEvent[] events)
        {
           _uncommittedEvents.AddRange(events);
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