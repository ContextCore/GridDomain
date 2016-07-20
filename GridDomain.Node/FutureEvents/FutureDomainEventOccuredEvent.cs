using System;
using GridDomain.EventSourcing;

namespace GridDomain.Node.FutureEvents
{
    public class FutureDomainEventOccuredEvent : DomainEvent
    {
        public FutureDomainEventOccuredEvent(Guid sourceId)
            : base(sourceId)
        {
        }
    }
}