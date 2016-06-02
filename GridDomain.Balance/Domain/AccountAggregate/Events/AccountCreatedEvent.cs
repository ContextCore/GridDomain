using System;
using GridDomain.EventSourcing;

namespace GridDomain.Balance.Domain.AccountAggregate.Events
{
    public class AccountCreatedEvent : DomainEvent
    {
        public AccountCreatedEvent(Guid balanceId, Guid businessId) : base(balanceId)
        {
            BusinessId = businessId;
        }

        public Guid BalanceId => SourceId;
        public Guid BusinessId { get; }
    }
}