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
    public abstract class  Aggregate : IAggregate,
                                      IMemento,
                                      IEquatable<IAggregate>
    {

        private readonly List<DomainEvent> _uncommittedEvents = new List<DomainEvent>(7);

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

        void IAggregate.Apply(DomainEvent @event)
        {
            OnAppyEvent(@event);
            Version++;
        }

        protected abstract void OnAppyEvent(DomainEvent evt);

        public IReadOnlyCollection<DomainEvent> GetUncommittedEvents()
        {
            return  _uncommittedEvents;
        }

        public void ClearUncommitedEvents()
        {
            _uncommittedEvents.Clear();
        }

        public virtual bool Equals(IAggregate other)
        {
            return null != other && other.Id == Id;
        }

        protected async Task Emit<T>(Task<T> evtTask) where T : DomainEvent
        {
             Emit(await evtTask);
        }

       
        protected void Emit(params DomainEvent[] events)
        {
            _uncommittedEvents.AddRange(events);
            foreach (var e in events) ((IAggregate) this).Apply(e);
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