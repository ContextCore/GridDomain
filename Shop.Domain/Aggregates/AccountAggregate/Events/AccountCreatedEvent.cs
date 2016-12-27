using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.AccountAggregate.Events
{
    public class AccountCreatedEvent : DomainEvent
    {
        public int AccountNumber { get; }
        public Guid UserId { get; }

        public AccountCreatedEvent(Guid sourceId, Guid userId, int accountNumber) : base(sourceId)
        {
            AccountNumber = accountNumber;
            UserId = userId;
        }

    }
}