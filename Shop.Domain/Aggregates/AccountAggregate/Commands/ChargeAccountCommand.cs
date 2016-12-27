using System;
using GridDomain.CQRS;
using NMoneys;

namespace Shop.Domain.Aggregates.AccountAggregate.Commands
{
    public class ChargeAccountCommand : Command
    {
        protected ChargeAccountCommand(Guid accountId, Money amount)
        {
            AccountId = accountId;
            Amount = amount;
        }

        public Guid AccountId { get; }
        public Money Amount { get; }
    }
}