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
                                      ISnapshot,
                                      IEquatable<IAggregate>,
                                      IEventList
                                      
    {
        protected readonly List<DomainEvent> _uncommittedEvents = new List<DomainEvent>(7);

        protected Aggregate(string id)
        {
            Id = id;
        }

        string ISnapshot.Id
        {
            get => Id;
            set => Id = value;
        }

        int ISnapshot.Version
        {
            get => Version;
            set => Version = value;
        }

        public string Id { get; protected set; }
        public int Version { get; protected set; }

        void IEventSourced.Apply(DomainEvent @event)
        {
            OnApplyEvent(@event);
            Version++;
        }

        protected abstract void OnApplyEvent(DomainEvent evt);

        public IReadOnlyCollection<DomainEvent> Events => _uncommittedEvents;

        public void Clear()
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
            foreach (var e in events) 
                ((IAggregate) this).Apply(e);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public void OnPersist(DomainEvent ev)
        {
         //   ((IAggregate) this).Apply(ev);
        }

        public abstract Task<IReadOnlyCollection<DomainEvent>> Execute(ICommand command);

        public override bool Equals(object obj)
        {
            return Equals(obj as IAggregate);
        }
    }
}