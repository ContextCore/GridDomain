using System;
using GridDomain.EventSourcing;

namespace Shop.Domain.Aggregates.OrderAggregate.Events
{
    internal class OrderCreated : DomainEvent
    {
        public int Number { get; }
        public Guid Id { get; }
        public Guid User { get; }

        public OrderCreated(Guid id, int number, Guid user):base(id)
        {
            Id = id;
            User = user;
            Number = number;
        }
    
    }
}