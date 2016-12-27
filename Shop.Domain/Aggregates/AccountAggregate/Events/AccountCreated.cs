using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.AccountAggregate.Events
{
    public class AccountCreated : DomainEvent
    {
        public int AccountNumber { get; }
        public Guid UserId { get; }

        public AccountCreated(Guid sourceId, Guid userId, int accountNumber) : base(sourceId)
        {
            AccountNumber = accountNumber;
            UserId = userId;
        }

    }
}