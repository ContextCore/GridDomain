using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonDomain.Core;
using GridDomain.EventSourcing;

namespace GridDomain.Balance.Domain.BusinessAggregate
{
    class Business : AggregateBase
    {
        Guid BalanceId;
        Guid SubscriptionId;

        public string Name;

        public Business(Guid id, string name,Guid subscriptionId, Guid balanceId)
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
