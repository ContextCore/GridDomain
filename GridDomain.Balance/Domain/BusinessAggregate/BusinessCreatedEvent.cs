using System;
using GridDomain.EventSourcing;

namespace GridDomain.Balance.Domain.BusinessAggregate
{
    class BusinessCreatedEvent : DomainEvent
    {
        public Guid BalanceId;
        public Guid SubscriptionId;
        public string Names;

        public BusinessCreatedEvent(Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
        }
    }
}