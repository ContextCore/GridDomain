using System.Collections.Generic;

namespace GridDomain.EventSourcing.DomainEventAdapters
{
    public interface IDomainEventAdapter<TFrom,TTo>: IEventAdapter 
                                                    where TFrom:DomainEvent 
                                                    where TTo: DomainEvent
    {
        IEnumerable<TTo> ConvertEvent(TFrom evt);
    }
}