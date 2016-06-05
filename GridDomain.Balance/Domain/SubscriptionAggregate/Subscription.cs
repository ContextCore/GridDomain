using System;
using CommonDomain.Core;
using NMoneys;

namespace GridDomain.Balance.Domain.OfferAggregate
{
    internal class Subscription : AggregateBase
    {
        public Money Cost;
        public string[] Grants;
        public string Name;
        public TimeSpan Period;

        private Subscription(Guid id)
        {
            Id = id;
        }

        public Subscription(Guid id, TimeSpan period, Money cost, string name, string[] grants) : this(id)
        {
            RaiseEvent(new SubscriptionCreatedEvent(id)
            {
                Period = period,
                Cost = cost,
                Name = name,
                Grants = grants
            });
        }

        private void Apply(SubscriptionCreatedEvent e)
        {
            Id = e.SourceId;
            Period = e.Period;
            Cost = e.Cost;
            Grants = e.Grants;
            Name = e.Name;
        }
    }
}