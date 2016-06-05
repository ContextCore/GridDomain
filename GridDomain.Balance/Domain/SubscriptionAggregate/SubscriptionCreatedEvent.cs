using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace GridDomain.Balance.Domain.OfferAggregate
{
    internal class SubscriptionCreatedEvent : DomainEvent
    {
        public Money Cost;
        public string[] Grants;
        public string Name;
        public TimeSpan Period;

        public SubscriptionCreatedEvent(Guid sourceId, DateTime? createdTime = null)
            : base(sourceId, createdTime)
        {
        }
    }
}