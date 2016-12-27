using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.AccountAggregate.Events
{
    public class AccountCreatedEvent : DomainEvent
    {
        public AccountCreatedEvent(Guid sourceId, Guid businessId) : base(sourceId)
        {
            BusinessId = businessId;
        }

        public Guid BusinessId { get; }
    }
}