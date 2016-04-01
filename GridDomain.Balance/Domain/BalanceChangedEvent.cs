using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace GridDomain.Balance.Domain
{
    public class BalanceChangedEvent : DomainEvent
    {
        protected BalanceChangedEvent(Guid balanceId, Money amount) : base(balanceId)
        {
            Amount = amount;
        }

        public Guid BalanceId => SourceId;
        public Money Amount { get; private set; }
    }
}