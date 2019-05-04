using System.Collections.Generic;
using System.Linq;
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

        protected async Task<IReadOnlyCollection<IDomainEvent>> Emit<T>(Task<T> evtTask) where T : DomainEvent
        {
            return await Emit(await evtTask);
        }

        protected Task<IReadOnlyCollection<IDomainEvent>> Emit(params IDomainEvent[] events)
        {
            foreach (var e in events)
            {
                e.Version = Version;
                ((IEventSourced) this).Apply(e);
            }

            return events.AsCommandResult();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public abstract Task<IReadOnlyCollection<IDomainEvent>> Execute(ICommand command);

        public override bool Equals(object obj)
        {
            return Equals(obj as IAggregate);
        }
    }
}