using System;
using CommonDomain.Core;

namespace GridDomain.Balance.Domain.BusinessAggregate
{
    internal class Business : AggregateBase
    {
        private Guid BalanceId;

        public string Name;
        private Guid SubscriptionId;

        public Business(Guid id, string name, Guid subscriptionId, Guid balanceId)
        {
            RaiseEvent(new BusinessCreatedEvent(id)
            {
                Names = name,
                SubscriptionId = subscriptionId,
                BalanceId = balanceId
            });
        }

        private void Apply(BusinessCreatedEvent e)
        {
            Id = e.SourceId;
            BalanceId = e.BalanceId;
            SubscriptionId = e.SubscriptionId;
        }
    }
}