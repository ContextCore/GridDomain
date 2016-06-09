using System;
using GridDomain.EventSourcing;
using NMoneys;

namespace BusinessNews.Domain.AccountAggregate.Events
{
    public class AccountBalanceChangedEvent : DomainEvent
    {
        protected AccountBalanceChangedEvent(Guid accountId, Money amount) : base(accountId)
        {
            Amount = amount;
        }

        public Guid BalanceId => SourceId;
        public Money Amount { get; private set; }
    }
}