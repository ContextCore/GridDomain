using System.Collections.Generic;
using System.Linq;

namespace GridDomain.EventSourcing.Adapters
{
    public abstract class DomainEventAdapter<TFrom, TTo> : IDomainEventAdapter<TFrom, TTo> where TFrom : DomainEvent where TTo : DomainEvent
    {
        public abstract IEnumerable<TTo> ConvertEvent(TFrom evt);

        IEnumerable<object> IEventAdapter.Convert(object evt)
        {
            return ConvertEvent((TFrom) evt);
        }
    }
}