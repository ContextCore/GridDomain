using System;
using GridDomain.EventSourcing;

namespace GridDomain.Balance.Domain.ServiceSubscriptionAggregate
{
    class GrantCreatedEvent : DomainEvent
    {
        public string Value;
        public GrantCreatedEvent(Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
        }
    }
}