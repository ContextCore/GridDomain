using System;
using CommonDomain.Core;

namespace GridDomain.Balance.Domain.ServiceSubscriptionAggregate
{
    class SubscriptionRights : AggregateBase
    {
        public string Name { get; }
        public Grant[] Rights { get; }

        private SubscriptionRights(Guid id)
        {
            Id = id;
        }

        public SubscriptionRights(Guid id, string name, Grant[] grants):this(id)
        {
            RaiseEvent(new SubscriptionRightsCreatedEvent(id)
            {
                Name = name,Rights = grants
            });
        }
    }
}