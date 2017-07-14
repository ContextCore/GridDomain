using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Unit.FutureEvents.Infrastructure {
    public class BoomDomainEvent : DomainEvent
    {
        public BoomDomainEvent(Guid sourceId) : base(sourceId)
        {

        }
    }
}