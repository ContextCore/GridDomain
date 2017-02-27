using System;
using GridDomain.CQRS;
using NMoneys;

namespace Shop.Domain.Aggregates.AccountAggregate.Commands
{
    public class ChargeAccountCommand : Command
    {
        protected ChargeAccountCommand(Guid accountId, Money amount):base(accountId)
        {
            Amount = amount;
        }

        public Guid AccountId => AggregateId;
        public Money Amount { get; }
    }
}