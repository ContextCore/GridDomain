using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.UserAggregate.Commands
{
    class CreateUserCommand : Command
    {
        public CreateUserCommand(Guid userId, string login, Guid accountId)
        {
            UserId = userId;
            Login = login;
            AccountId = accountId;
        }

        public Guid UserId { get; }
        public string Login { get; }
        public Guid AccountId { get; }
    }
}
