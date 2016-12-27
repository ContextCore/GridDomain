using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace Shop.Domain.Aggregates.AccountAggregate.Events
{
    public class AccountAmountChanged : DomainEvent
    {
        protected AccountAmountChanged(Guid sourceId, Money amount) : base(sourceId)
        {
            Amount = amount;
        }

        public Money Amount { get; }
    }
}