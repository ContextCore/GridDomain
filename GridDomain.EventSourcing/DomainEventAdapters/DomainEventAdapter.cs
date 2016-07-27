using System.Collections.Generic;
using System.Linq;
using CommonDomain;

namespace GridDomain.EventSourcing.VersionedTypeSerialization
{
    public abstract class DomainEventAdapter<TFrom, TTo> : IDomainEventAdapter<TFrom, TTo> where TFrom : DomainEvent where TTo : DomainEvent
    {
        public abstract IEnumerable<TTo> ConvertEvent(TFrom evt);

        public EventAdapterDescriptor Descriptor { get; } = new EventAdapterDescriptor(typeof(TFrom), typeof(TTo));

        IEnumerable<object> IEventAdapter.Convert(object evt)
        {
            IEnumerable<TTo> updatedEvents = ConvertEvent((TFrom)evt);
            return updatedEvents.Cast<object>();
        }
    }
}