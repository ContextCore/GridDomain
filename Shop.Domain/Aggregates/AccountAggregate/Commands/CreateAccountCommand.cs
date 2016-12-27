using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.AccountAggregate.Commands
{
    public class CreateAccountCommand : Command
    {
        public CreateAccountCommand(Guid accountId, Guid userId, int number)
        {
            AccountId = accountId;
            UserId = userId;
            Number = number;
        }

        public Guid AccountId { get; }
        public Guid UserId { get;}
        public int Number { get;}
    }
}