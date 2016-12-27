using System;
using GridDomain.EventSourcing;
using Shop.Domain.Aggregates.UserAggregate;

namespace Shop.Domain.Aggregates.OrderAggregate.Events
{
    public class OrderCreated : DomainEvent
    {
        public int Number { get; }
        public Guid User { get; }

        public OrderStatus Status { get; }

        public OrderCreated(Guid sourceId, int number, Guid user, OrderStatus status = OrderStatus.Created):base(sourceId)
        {
            Status = status;
            User = user;
            Number = number;
        }
    
    }
}