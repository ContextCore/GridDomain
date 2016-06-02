using System;
using CommonDomain.Core;

namespace GridDomain.Balance.Domain.ServiceSubscriptionAggregate
{
    class Grant : AggregateBase
    {
        public string Value;
        private Grant(Guid id)
        {
            Id = id;
        }

        public Grant(Guid id, string value):this(id)
        {
            RaiseEvent(new GrantCreatedEvent(id) {Value = value});
        }

        private void Apply(GrantCreatedEvent e)
        {
            Id = e.SourceId;
            Value = e.Value;
        }
    }
}