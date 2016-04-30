using System;
using GridDomain.EventSourcing;

namespace GridDomain.Balance.Domain.ServiceSubscriptionAggregate
{
    class SubscriptionRightsCreatedEvent : DomainEvent
    {

        public Guid Id;
        public string Name;
        public Grant[] Rights;

        public SubscriptionRightsCreatedEvent(Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
        }
    }
}