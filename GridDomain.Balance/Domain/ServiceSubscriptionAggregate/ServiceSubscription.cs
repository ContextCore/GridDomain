using System;
using CommonDomain.Core;
using NMoneys;

namespace GridDomain.Balance.Domain.ServiceSubscriptionAggregate
{
    internal class ServiceSubscription : AggregateBase
    {
        public Money Cost;
        public string[] Grants;
        public string Name;
        public TimeSpan Period;

        private ServiceSubscription(Guid id)
        {
            Id = id;
        }

        public ServiceSubscription(Guid id, TimeSpan period, Money cost, string name, string[] grants) : this(id)
        {
            RaiseEvent(new ServiceSubscriptionCreatedEvent(id)
            {
                Period = period,
                Cost = cost,
                Name = name,
                Grants = grants
            });
        }

        private void Apply(ServiceSubscriptionCreatedEvent e)
        {
            Id = e.SourceId;
            Period = e.Period;
            Cost = e.Cost;
            Grants = e.Grants;
            Name = e.Name;
        }
    }
}