using System;
using GridDomain.CQRS;

namespace Shop.Domain.Aggregates.OrderAggregate.Commands
{
    public class OrderCommand : Command
    {
        public OrderCommand(Guid orderId) : base(orderId) {}

        public Guid OrderId => AggregateId;
    }
}