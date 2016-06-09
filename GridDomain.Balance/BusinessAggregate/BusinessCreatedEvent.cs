using System;
using GridDomain.EventSourcing;

namespace BusinessNews.Domain.BusinessAggregate
{
    public class BusinessCreatedEvent : DomainEvent
    {
        public Guid AccountId;
        public string Name;
        public Guid SubscriptionId;

        public BusinessCreatedEvent(Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
        }
    }
}