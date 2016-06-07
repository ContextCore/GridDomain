using System;
using GridDomain.CQRS;
using NMoneys;

namespace BusinessNews.Domain.AccountAggregate.Commands
{
    public class ChangeAccountCommand : Command
    {
        public ChangeAccountCommand(Guid accountId, Money amount)
        {
            AccountId = accountId;
            Amount = amount;
        }

        public Guid AccountId { get; private set; }
        public Money Amount { get; private set; }
    }
}