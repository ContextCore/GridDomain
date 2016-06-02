using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace GridDomain.Balance.Domain.OfferAggregate
{
    internal class ServiceSubscriptionCreatedEvent : DomainEvent
    {
        public Money Cost;
        public string[] Grants;
        public string Name;
        public TimeSpan Period;

        public ServiceSubscriptionCreatedEvent(Guid sourceId, DateTime? createdTime = null)
            : base(sourceId, createdTime)
        {
        }
    }
}