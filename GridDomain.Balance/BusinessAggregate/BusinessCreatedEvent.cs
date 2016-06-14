using System;
using GridDomain.EventSourcing;

namespace BusinessNews.Domain.BusinessAggregate
{
    public class BusinessCreatedEvent : DomainEvent
    {
        public Guid BalanceId;
        public string Names;
        public Guid SubscriptionId;

        public BusinessCreatedEvent(Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
        }
    }
}