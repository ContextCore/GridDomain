using System.Collections.Generic;
using System.Linq;
using CommonDomain;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    public abstract class DomainEventAdapter<TAggregate, TFrom, TTo> : IDomainEventAdapter<TAggregate, TFrom, TTo>
    {
        public abstract IEnumerable<TTo> ConvertEvent(TAggregate aggregate, TFrom evt);

        public AdapterDescriptor Descriptor { get; } = new AdapterDescriptor(typeof(TFrom), typeof(TTo));

        IEnumerable<object> IDomainEventAdapter.Convert(IAggregate aggregate, object evt)
        {
            IEnumerable<TTo> updatedEvents = ConvertEvent((TAggregate)aggregate, (TFrom)evt);
            return updatedEvents.Cast<object>();
        }
    }
}