using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace Shop.Domain.Aggregates.AccountAggregate.Events
{
    public class AccountAmountChanged : DomainEvent
    {
        protected AccountAmountChanged(Guid sourceId, Guid changeId, Money amount) : base(sourceId)
        {
            Amount = amount;
            ChangeId = changeId;
        }

        public Money Amount { get; }

        public Guid ChangeId { get; }
    }
}