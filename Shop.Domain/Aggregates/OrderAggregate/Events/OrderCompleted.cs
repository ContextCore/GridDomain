using System;
using GridDomain.EventSourcing;
using Shop.Domain.Aggregates.UserAggregate;

namespace Shop.Domain.Aggregates.OrderAggregate.Events
{
    public class OrderCompleted : DomainEvent
    {
        public OrderCompleted(Guid id, OrderStatus status) : base(id)
        {
            Status = status;
        }

        public OrderStatus Status { get; }
    }
}