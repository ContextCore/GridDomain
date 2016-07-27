using System.Collections.Generic;
using System.Linq;
using CommonDomain;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    public abstract class DomainEventAdapter<TFrom, TTo> : IDomainEventAdapter<TFrom, TTo>
    {
        public abstract IEnumerable<TTo> ConvertEvent(TFrom evt);

        public AdapterDescriptor Descriptor { get; } = new AdapterDescriptor(typeof(TFrom), typeof(TTo));

        IEnumerable<object> IDomainEventAdapter.Convert(object evt)
        {
            IEnumerable<TTo> updatedEvents = ConvertEvent((TFrom)evt);
            return updatedEvents.Cast<object>();
        }
    }
}