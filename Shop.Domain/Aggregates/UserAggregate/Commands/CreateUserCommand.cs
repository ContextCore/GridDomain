using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.UserAggregate.Commands
{
    internal class CreateUserCommand : Command
    {
        public CreateUserCommand(Guid userId, string login, Guid accountId) : base(userId)
        {
            Login = login;
            AccountId = accountId;
        }

        public Guid UserId => AggregateId;
        public string Login { get; }
        public Guid AccountId { get; }
    }
}