using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.UserAggregate.Events
{
    internal class UserCreated:DomainEvent
    {
        public Guid Id { get; }
        public string Login { get;}
        public Guid Account { get;}

        public UserCreated(Guid id, string login, Guid account):base(id)
        {
            Id = id;
            Login = login;
            Account = account;
        }
    }
}