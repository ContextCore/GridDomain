using System;
using GridDomain.EventSourcing;

namespace GridDomain.Tests.Sagas.InstanceSagas.Events
{
    public class GotTiredDomainEvent: DomainEvent
    {
        public GotTiredDomainEvent(Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
        }
    }
}