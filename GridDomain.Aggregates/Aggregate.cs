using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridDomain.Aggregates
{
    public abstract class Aggregate : IAggregate
    {
        protected Aggregate(string id)
        {
            Id = id;
        }

        public string Id { get; protected set; }
        public int Version { get; protected set; }

        void IEventSourced.Apply(IDomainEvent @event)
        {
            OnApplyEvent(@event);
            Version++;
        }

        protected abstract void OnApplyEvent(IDomainEvent evt);


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

        public abstract Task<IReadOnlyCollection<IDomainEvent>> Execute(ICommand command);

        public override bool Equals(object obj)
        {
            return Equals(obj as IAggregate);
        }
    }
}