using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace GridDomain.Balance.Domain.ServiceSubscriptionAggregate
{
    class ServiceSubscriptionCreatedEvent : DomainEvent
    {
        public TimeSpan Period;
        public Money Cost;
        public string[] Grants;
        public string Name;

        public ServiceSubscriptionCreatedEvent(Guid sourceId, DateTime? createdTime = null) : base(sourceId, createdTime)
        {
        }
    }
}