using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.UserAggregate.Events
{
    internal class UserCreated : DomainEvent
    {
        public UserCreated(Guid id, string login, Guid account) : base(id)
        {
            Login = login;
            Account = account;
        }

        public string Login { get; }
        public Guid Account { get; }
    }
}